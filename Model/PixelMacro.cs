using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;
using System.Threading;
using System.Windows.Forms;
using System.Windows.Input;
using Newtonsoft.Json;
using _4RTools.Utils;

namespace _4RTools.Model
{
    public class PixelMacroRule
    {
        public int id { get; set; }
        public string name { get; set; } = "";
        public bool enabled { get; set; }
        public int red { get; set; }
        public int green { get; set; }
        public int blue { get; set; }
        public bool hasColor { get; set; }
        public int tolerance { get; set; } = 5;
        public Key key { get; set; } = Key.None;
        public int delay { get; set; } = 250;

        public PixelMacroRule() { }

        public PixelMacroRule(int id)
        {
            this.id = id;
        }
    }

    public class PixelMacro : Action
    {
        public static string ACTION_NAME_PIXEL_MACRO = "PixelMacro";

        private Thread thread;
        private readonly ManualResetEventSlim stopSignal = new ManualResetEventSlim(true);
        private readonly object mouseLock = new object();
        private bool mouseHeld;
        private int heldRuleId;
        private IntPtr heldWindow;
        private Point heldPosition;
        private bool holdRequested;
        private bool pixelWasDetected;
        private DateTime attackCursorStationarySinceUtc;
        private Point attackCursorPosition;
        private DateTime lastFollowUpSentUtc;
        private bool ruleEnableStateInitialized;
        private bool scanFailed;
        private bool thirdPixelWasDetected;
        private readonly System.Diagnostics.Stopwatch thirdPixelTimer = new System.Diagnostics.Stopwatch();
        private bool thirdPixelMacroSent;
        private bool mapPixelWasDetected;
        private readonly System.Diagnostics.Stopwatch mapMacroTimer = new System.Diagnostics.Stopwatch();
        private readonly System.Diagnostics.Stopwatch mapClickTimer = new System.Diagnostics.Stopwatch();
        private readonly System.Diagnostics.Stopwatch randomClickTimer = new System.Diagnostics.Stopwatch();
        private readonly Random randomClickPosition = new Random();
        public bool enabled { get; set; } = false;
        public bool useSearchArea { get; set; }
        public int searchX1 { get; set; } = 500;
        public int searchY1 { get; set; } = 500;
        public int searchX2 { get; set; } = 900;
        public int searchY2 { get; set; } = 900;
        public int blockSize { get; set; } = 4;
        public int scanDelay { get; set; } = 10;
        public Key followUpKey { get; set; } = Key.None;
        public List<PixelMacroRule> rules { get; set; } = new List<PixelMacroRule>();

        public PixelMacro()
        {
            this.rules.Add(CreateDefaultRule(1));
            this.rules.Add(CreateDefaultRule(2));
            this.rules.Add(CreateDefaultRule(3));
            this.rules.Add(CreateDefaultRule(4));
        }

        public string GetActionName()
        {
            return ACTION_NAME_PIXEL_MACRO;
        }

        public string GetConfiguration()
        {
            return JsonConvert.SerializeObject(this);
        }

        public void Start()
        {
            Stop();
            if (this.thread != null && this.thread.IsAlive) { return; }
            EnsureRules();
            if (!this.enabled)
            {
                return;
            }

            Client roClient = ClientSingleton.GetClient();
            if (roClient != null)
            {
                this.pixelWasDetected = false;
                this.lastFollowUpSentUtc = DateTime.MinValue;
                this.thirdPixelTimer.Reset();
                this.mapPixelWasDetected = false;
                this.thirdPixelWasDetected = false;
                this.thirdPixelMacroSent = false;
                this.stopSignal.Reset();
                this.randomClickTimer.Restart();
                this.thread = new Thread(() =>
                {
                    try
                    {
                        while (!this.stopSignal.IsSet)
                        {
                            this.holdRequested = false;
                            try
                            {
                                PixelMacroExecutionThread(roClient);
                                RandomClickIfDue(roClient);
                            }
                            catch (Exception ex)
                            {
                                ReleaseMouseHold();
                                System.Diagnostics.Debug.WriteLine(ex);
                            }
                            finally { if (!this.holdRequested) { ReleaseMouseHold(); } }
                            WaitForNextScan(5);
                        }
                    }
                    finally { ReleaseMouseHold(); }
                }) { IsBackground = true };
                this.thread.SetApartmentState(ApartmentState.STA);
                Application.ApplicationExit += OnApplicationExit;
                this.thread.Start();
            }
        }

        public void Stop()
        {
            this.stopSignal.Set();
            if (this.thread != null && this.thread != Thread.CurrentThread && this.thread.IsAlive)
            {
                this.thread.Join(1000);
            }
            ReleaseMouseHold();
            Application.ApplicationExit -= OnApplicationExit;
        }

        private void OnApplicationExit(object sender, EventArgs e) { Stop(); }

        private void WaitForNextScan(int milliseconds)
        {
            while (milliseconds > 0 && !this.stopSignal.IsSet)
            {
                int interval = Math.Min(25, milliseconds);
                if (this.stopSignal.Wait(interval)) { return; }
                lock (this.mouseLock)
                {
                    if (this.mouseHeld && (!this.enabled || (this.heldRuleId > 0 && !this.rules[this.heldRuleId - 1].enabled)
                        || !CanClickAt(this.heldWindow, new Interop.POINT { X = this.heldPosition.X, Y = this.heldPosition.Y })
                        || System.Windows.Forms.Cursor.Position != this.heldPosition)) { ReleaseMouseHold(); }
                }
                milliseconds -= interval;
            }
        }

        public void EnsureRules()
        {
            if (this.rules == null)
            {
                this.rules = new List<PixelMacroRule>();
            }

            while (this.rules.Count < 4)
            {
                this.rules.Add(CreateDefaultRule(this.rules.Count + 1));
            }
            if (this.rules.Count > 4)
            {
                this.rules.RemoveRange(4, this.rules.Count - 4);
            }

            this.rules[0].id = 1;
            this.rules[1].id = 2;
            this.rules[0].name = "Attack Pixel";
            this.rules[1].name = "Evade Pixel";
            this.rules[2].id = 3;
            this.rules[2].name = "8-second Pixel";
            this.rules[3].id = 4;
            this.rules[3].name = "Map Pixel";
            ApplyDefaultColorIfUnconfigured(this.rules[0], 0x39, 0x4A, 0xCE);
            ApplyDefaultColorIfUnconfigured(this.rules[1], 0xFF, 0x00, 0x00);
            ApplyDefaultColorIfUnconfigured(this.rules[2], 0x53, 0x75, 0x59);
            ApplyDefaultColorIfUnconfigured(this.rules[3], 0xFF, 0xF7, 0x00);

            if (!this.ruleEnableStateInitialized)
            {
                this.enabled = false;
                this.rules[0].enabled = false;
                this.rules[1].enabled = false;
                this.rules[2].enabled = false;
                this.rules[3].enabled = false;
                this.ruleEnableStateInitialized = true;
            }

            foreach (PixelMacroRule rule in this.rules)
            {
                if (!rule.hasColor && (rule.red != 0 || rule.green != 0 || rule.blue != 0))
                {
                    rule.hasColor = true;
                }
            }
        }

        private static PixelMacroRule CreateDefaultRule(int id)
        {
            PixelMacroRule rule = new PixelMacroRule(id) { enabled = false, hasColor = true };
            if (id == 1)
            {
                rule.name = "Attack Pixel";
                rule.red = 0x39;
                rule.green = 0x4A;
                rule.blue = 0xCE;
            }
            else if (id == 2)
            {
                rule.name = "Evade Pixel";
                rule.red = 0xFF;
                rule.green = 0x00;
                rule.blue = 0x00;
            }
            else if (id == 3)
            {
                rule.name = "8-second Pixel";
                rule.red = 0x53;
                rule.green = 0x75;
                rule.blue = 0x59;
            }
            else
            {
                rule.name = "Map Pixel";
                rule.red = 0xFF;
                rule.green = 0xF7;
                rule.blue = 0x00;
            }
            return rule;
        }

        private static void ApplyDefaultColorIfUnconfigured(PixelMacroRule rule, int red, int green, int blue)
        {
            if (!rule.hasColor && rule.red == 0 && rule.green == 0 && rule.blue == 0)
            {
                rule.red = red;
                rule.green = green;
                rule.blue = blue;
                rule.hasColor = true;
            }
        }

        private int PixelMacroExecutionThread(Client roClient)
        {
            this.scanFailed = false;
            if (this.stopSignal.IsSet || !this.enabled || !IsTargetAvailable(roClient)
                || Interop.GetForegroundWindow() != roClient.process.MainWindowHandle)
            {
                ReleaseMouseHold();
                this.mapPixelWasDetected = false;
                this.pixelWasDetected = false;
                this.thirdPixelWasDetected = false;
                this.thirdPixelMacroSent = false;
                WaitForNextScan(30);
                return 0;
            }

            // Map Pixel takes priority over every other rule until it disappears.
            PixelMacroRule mapRule = this.rules[3];
            Point mapScreenLocation;
            Point mapClientLocation;
            if (mapRule.enabled && mapRule.hasColor
                && TryFindMatchingPixel(roClient, mapRule, out mapScreenLocation, out mapClientLocation))
            {
                this.pixelWasDetected = false;
                this.thirdPixelWasDetected = false;
                this.thirdPixelMacroSent = false;
                bool firstDetection = !this.mapPixelWasDetected;
                this.mapPixelWasDetected = true;
                if (firstDetection || this.mapMacroTimer.ElapsedMilliseconds >= 300)
                {
                    ActivateFollowUpMacro(roClient);
                    this.mapMacroTimer.Restart();
                }
                // Recheck the target before each scheduled double-click.
                if ((firstDetection || this.mapClickTimer.ElapsedMilliseconds >= 500)
                    && TryFindMatchingPixel(roClient, mapRule, out mapScreenLocation, out mapClientLocation))
                {
                    this.mapClickTimer.Restart();
                    ClickPixel(roClient, mapClientLocation, 4);
                }
                else { ReleaseMouseHold(); }
                WaitForNextScan(Math.Max(10, scanDelay));
                return 0;
            }
            this.mapPixelWasDetected = false;
            if (this.heldRuleId == 4) { ReleaseMouseHold(); }

            bool detected = false;
            bool yieldAttackToThirdPixel = false;

            PixelMacroRule macroPixelRule = this.rules[1];
            if (macroPixelRule.enabled && macroPixelRule.hasColor)
            {
                Point macroScreenLocation;
                Point macroClientLocation;
                if (TryFindMatchingPixel(roClient, macroPixelRule, out macroScreenLocation, out macroClientLocation))
                {
                    ReleaseMouseHold();
                    this.pixelWasDetected = false;
                    this.thirdPixelWasDetected = false;
                    this.thirdPixelMacroSent = false;
                    ActivateFollowUpMacro(roClient);
                    WaitForNextScan(300);
                    return 0;
                }
            }

            PixelMacroRule rule = this.rules[0];
            if (rule.enabled && rule.hasColor)
            {
                Point matchScreenLocation;
                Point matchClientLocation;
                if (TryFindMatchingPixel(roClient, rule, out matchScreenLocation, out matchClientLocation))
                {
                    detected = true;
                    Point cursorPosition = System.Windows.Forms.Cursor.Position;
                    // A detected Attack target takes over from the third rule with a fresh timer.
                    if (!this.pixelWasDetected || this.thirdPixelWasDetected
                        || cursorPosition != this.attackCursorPosition)
                    {
                        this.attackCursorPosition = cursorPosition;
                        this.attackCursorStationarySinceUtc = DateTime.UtcNow;
                    }
                    this.pixelWasDetected = true;
                    if (!IsTargetAvailable(roClient)) { return 0; }
                    yieldAttackToThirdPixel = DateTime.UtcNow - this.attackCursorStationarySinceUtc
                        >= TimeSpan.FromSeconds(10);
                    if (!yieldAttackToThirdPixel)
                    {
                        ClickPixel(roClient, matchClientLocation, 1);
                        WaitForNextScan(Math.Max(1, rule.delay));
                    }
                }
            }

            if (!detected)
            {
                this.pixelWasDetected = false;
            }

            // While the third pixel is visible, suppress the no-attack macro fallback.
            if ((!detected || yieldAttackToThirdPixel) && this.heldRuleId == 1) { ReleaseMouseHold(); }
            PixelMacroRule thirdRule = this.rules[2];
            Point thirdScreenLocation;
            Point thirdClientLocation;
            if ((!detected || yieldAttackToThirdPixel) && thirdRule.enabled && thirdRule.hasColor
                && TryFindMatchingPixel(roClient, thirdRule, out thirdScreenLocation, out thirdClientLocation))
            {
                if (!IsTargetAvailable(roClient)) { return 0; }
                if (!ClickPixel(roClient, thirdClientLocation, 3))
                {
                    this.thirdPixelWasDetected = false;
                    this.thirdPixelMacroSent = false;
                    this.thirdPixelTimer.Reset();
                    WaitForNextScan(Math.Max(10, scanDelay));
                    return 0;
                }
                if (!this.thirdPixelWasDetected)
                {
                    this.thirdPixelTimer.Restart();
                    this.thirdPixelMacroSent = false;
                    this.thirdPixelWasDetected = true;
                }
                else if (!this.thirdPixelMacroSent
                    && this.thirdPixelTimer.Elapsed >= TimeSpan.FromSeconds(8))
                {
                    ActivateFollowUpMacro(roClient);
                    this.thirdPixelMacroSent = true;
                }

                WaitForNextScan(Math.Max(1, thirdRule.delay));
                WaitForNextScan(Math.Max(10, scanDelay));
                return 0;
            }
            this.thirdPixelWasDetected = false;
            this.thirdPixelMacroSent = false;
            if (this.heldRuleId == 3) { ReleaseMouseHold(); }

            if (!detected)
            {
                // Only a valid scan with no matching targets may use the idle fallback.
                if (this.scanFailed)
                {
                    WaitForNextScan(Math.Max(10, scanDelay));
                    return 0;
                }
                if (DateTime.UtcNow - this.lastFollowUpSentUtc >= TimeSpan.FromMilliseconds(300))
                {
                    ActivateFollowUpMacro(roClient);
                    this.lastFollowUpSentUtc = DateTime.UtcNow;
                }
                WaitForNextScan(Math.Max(10, scanDelay));
                return 0;
            }

            WaitForNextScan(Math.Max(10, scanDelay));
            return 0;
        }

        private void ActivateFollowUpMacro(Client roClient)
        {
            if (this.stopSignal.IsSet || this.scanFailed || this.followUpKey == Key.None || !IsTargetAvailable(roClient)
                || Interop.GetForegroundWindow() != roClient.process.MainWindowHandle)
            {
                return;
            }

            IntPtr windowHandle = roClient.process.MainWindowHandle;
            Keys key = (Keys)Enum.Parse(typeof(Keys), this.followUpKey.ToString());
            Interop.PostMessage(windowHandle, Constants.WM_KEYDOWN_MSG_ID, key, 0);
            Interop.PostMessage(windowHandle, Constants.WM_KEYUP_MSG_ID, key, 0);
        }

        public bool TryFindMatchingPixel(Client roClient, PixelMacroRule rule, out Point matchScreenLocation, out Point matchClientLocation)
        {
            Rectangle bounds = GetClientScreenBounds(roClient);
            if (bounds.Width <= 0 || bounds.Height <= 0)
            {
                this.scanFailed = true;
                matchScreenLocation = Point.Empty;
                matchClientLocation = Point.Empty;
                return false;
            }

            Rectangle scanBounds = bounds;
            if (rule.id == 2)
            {
                int width = Math.Max(1, bounds.Width / 4);
                int height = Math.Max(1, bounds.Height / 4);
                scanBounds = new Rectangle(bounds.Left + (bounds.Width - width) / 2,
                    bounds.Top + (bounds.Height - height) / 2, width, height);
            }

            // PrintWindow captures the full client; crop afterward to preserve coordinates.
            using (Bitmap captured = CaptureClientArea(roClient, bounds))
            using (Bitmap cropped = captured != null && rule.id == 2
                ? captured.Clone(new Rectangle(scanBounds.Left - bounds.Left, scanBounds.Top - bounds.Top,
                    scanBounds.Width, scanBounds.Height), PixelFormat.Format32bppArgb) : null)
            {
                Bitmap bitmap = cropped ?? captured;
                if (bitmap == null)
                {
                    this.scanFailed = true;
                    matchScreenLocation = Point.Empty;
                    matchClientLocation = Point.Empty;
                    return false;
                }
                int matchX;
                int matchY;
                int size = Math.Max(1, blockSize);
                if (bitmap.Width < size || bitmap.Height < size)
                {
                    this.scanFailed = true;
                    matchScreenLocation = Point.Empty;
                    matchClientLocation = Point.Empty;
                    return false;
                }

                int[] matchIntegral;
                byte[] matchMap = BuildMatchMap(bitmap, rule, out matchIntegral);
                if (TryFindBlockFromCenter(matchMap, matchIntegral, bitmap.Width, bitmap.Height, size, out matchX, out matchY))
                {
                    Point targetCenter = FindConnectedRegionCenter(
                        matchMap,
                        bitmap.Width,
                        bitmap.Height,
                        matchX + (size / 2),
                        matchY + (size / 2));
                    matchScreenLocation = new Point(
                        scanBounds.Left + targetCenter.X,
                        scanBounds.Top + targetCenter.Y);
                    matchClientLocation = new Point(
                        matchScreenLocation.X - bounds.Left,
                        matchScreenLocation.Y - bounds.Top);
                    return true;
                }
            }

            matchScreenLocation = Point.Empty;
            matchClientLocation = Point.Empty;
            return false;
        }

        private static bool TryFindBlockFromCenter(byte[] matchMap, int[] integral, int width, int height, int size, out int matchX, out int matchY)
        {
            int maxX = width - size;
            int maxY = height - size;
            int centerX = maxX / 2;
            int centerY = maxY / 2;
            if (TryBlockAt(integral, width, maxX, maxY, size, centerX, centerY, out matchX, out matchY))
            {
                return true;
            }

            // Expand in square rings instead of scanning an entire row before nearby rows.
            int maxRadius = Math.Max(Math.Max(centerX, maxX - centerX), Math.Max(centerY, maxY - centerY));
            for (int radius = 1; radius <= maxRadius; radius++)
            {
                int left = centerX - radius, right = centerX + radius;
                int top = centerY - radius, bottom = centerY + radius;
                for (int x = Math.Max(0, left); x <= Math.Min(maxX, right); x++)
                {
                    if (TryBlockAt(integral, width, maxX, maxY, size, x, top, out matchX, out matchY)
                        || TryBlockAt(integral, width, maxX, maxY, size, x, bottom, out matchX, out matchY)) { return true; }
                }
                for (int y = Math.Max(0, top + 1); y <= Math.Min(maxY, bottom - 1); y++)
                {
                    if (TryBlockAt(integral, width, maxX, maxY, size, left, y, out matchX, out matchY)
                        || TryBlockAt(integral, width, maxX, maxY, size, right, y, out matchX, out matchY)) { return true; }
                }
            }
            matchX = 0;
            matchY = 0;
            return false;
        }

        private static bool TryBlockAt(int[] integral, int width, int maxX, int maxY, int size, int x, int y, out int matchX, out int matchY)
        {
            matchX = x;
            matchY = y;
            return x >= 0 && x <= maxX && y >= 0 && y <= maxY
                && FullBlockMatches(integral, width, x, y, size);
        }

        public static bool IsTargetAvailable(Client client)
        {
            return client != null && client.process != null && !client.process.HasExited
                && client.process.MainWindowHandle != IntPtr.Zero
                && Interop.IsWindowVisible(client.process.MainWindowHandle)
                && !Interop.IsIconic(client.process.MainWindowHandle);
        }

        private static Bitmap CaptureClientArea(Client client, Rectangle bounds)
        {
            if (!IsTargetAvailable(client)) { return null; }
            Bitmap bitmap = new Bitmap(bounds.Width, bounds.Height, PixelFormat.Format32bppArgb);
            bool captured;
            using (Graphics graphics = Graphics.FromImage(bitmap))
            {
                IntPtr hdc = graphics.GetHdc();
                try
                {
                    // Capture only this window, even when another application has focus.
                    captured = Interop.PrintWindow(client.process.MainWindowHandle, hdc, 1);
                }
                finally { graphics.ReleaseHdc(hdc); }
            }
            if (!captured) { bitmap.Dispose(); return null; }
            return bitmap;
        }
        private static Rectangle GetClientScreenBounds(Client roClient)
        {
            Interop.RECT clientRect;
            if (!Interop.GetClientRect(roClient.process.MainWindowHandle, out clientRect))
            {
                return Rectangle.Empty;
            }

            Interop.POINT topLeft = new Interop.POINT { X = clientRect.Left, Y = clientRect.Top };
            if (!Interop.ClientToScreen(roClient.process.MainWindowHandle, ref topLeft))
            {
                return Rectangle.Empty;
            }

            return new Rectangle(
                topLeft.X,
                topLeft.Y,
                clientRect.Right - clientRect.Left,
                clientRect.Bottom - clientRect.Top);
        }

        private static bool ColorMatches(Color pixel, PixelMacroRule rule)
        {
            return Math.Abs(pixel.R - rule.red) <= rule.tolerance
                && Math.Abs(pixel.G - rule.green) <= rule.tolerance
                && Math.Abs(pixel.B - rule.blue) <= rule.tolerance;
        }

        private static byte[] BuildMatchMap(Bitmap bitmap, PixelMacroRule rule, out int[] integral)
        {
            int width = bitmap.Width;
            int height = bitmap.Height;
            byte[] matchMap = new byte[width * height];
            int integralWidth = width + 1;
            integral = new int[integralWidth * (height + 1)];
            Rectangle area = new Rectangle(0, 0, width, height);
            BitmapData data = bitmap.LockBits(area, ImageLockMode.ReadOnly, PixelFormat.Format32bppArgb);
            try
            {
                int stride = Math.Abs(data.Stride);
                byte[] pixels = new byte[stride * height];
                Marshal.Copy(data.Scan0, pixels, 0, pixels.Length);

                for (int y = 0; y < height; y++)
                {
                    int sourceRow = data.Stride >= 0 ? y * stride : (height - 1 - y) * stride;
                    for (int x = 0; x < width; x++)
                    {
                        int pixelIndex = sourceRow + (x * 4);
                        bool matches = Math.Abs(pixels[pixelIndex + 2] - rule.red) <= rule.tolerance
                            && Math.Abs(pixels[pixelIndex + 1] - rule.green) <= rule.tolerance
                            && Math.Abs(pixels[pixelIndex] - rule.blue) <= rule.tolerance;
                        if (matches) { matchMap[(y * width) + x] = 1; }
                        integral[((y + 1) * integralWidth) + x + 1] =
                            integral[(y * integralWidth) + x + 1]
                            + integral[((y + 1) * integralWidth) + x]
                            - integral[(y * integralWidth) + x]
                            + (matches ? 1 : 0);
                    }
                }
            }
            finally
            {
                bitmap.UnlockBits(data);
            }

            return matchMap;
        }

        private static bool FullBlockMatches(int[] integral, int width, int x, int y, int size)
        {
            int integralWidth = width + 1;
            int right = x + size;
            int bottom = y + size;
            int matches = integral[(bottom * integralWidth) + right]
                - integral[(y * integralWidth) + right]
                - integral[(bottom * integralWidth) + x]
                + integral[(y * integralWidth) + x];
            return matches == size * size;
        }

        private static Point FindConnectedRegionCenter(byte[] matchMap, int width, int height, int seedX, int seedY)
        {
            bool[] visited = new bool[matchMap.Length];
            Queue<int> pending = new Queue<int>();
            int seed = (seedY * width) + seedX;
            pending.Enqueue(seed);
            visited[seed] = true;
            int minX = seedX, maxX = seedX, minY = seedY, maxY = seedY;

            while (pending.Count > 0)
            {
                int index = pending.Dequeue();
                int x = index % width;
                int y = index / width;
                minX = Math.Min(minX, x); maxX = Math.Max(maxX, x);
                minY = Math.Min(minY, y); maxY = Math.Max(maxY, y);

                AddConnectedPixel(matchMap, visited, pending, width, height, x - 1, y);
                AddConnectedPixel(matchMap, visited, pending, width, height, x + 1, y);
                AddConnectedPixel(matchMap, visited, pending, width, height, x, y - 1);
                AddConnectedPixel(matchMap, visited, pending, width, height, x, y + 1);
            }

            Point center = new Point((minX + maxX) / 2, (minY + maxY) / 2);
            // A hollow or irregular region's bounding center may not match the color.
            return matchMap[(center.Y * width) + center.X] != 0
                ? center : new Point(seedX, seedY);
        }

        private static void AddConnectedPixel(byte[] map, bool[] visited, Queue<int> pending, int width, int height, int x, int y)
        {
            if (x < 0 || y < 0 || x >= width || y >= height) { return; }
            int index = (y * width) + x;
            if (visited[index] || map[index] == 0) { return; }
            visited[index] = true;
            pending.Enqueue(index);
        }

        private bool ClickPixel(Client roClient, Point clientLocation, int ruleId, int clickCount = 2)
        {
            if (this.stopSignal.IsSet || !this.enabled || !IsTargetAvailable(roClient)) { return false; }
            IntPtr handle = roClient.process.MainWindowHandle;
            Interop.RECT bounds;
            Interop.POINT point = new Interop.POINT { X = clientLocation.X, Y = clientLocation.Y };
            if (!Interop.GetClientRect(handle, out bounds)
                || clientLocation.X < bounds.Left || clientLocation.X >= bounds.Right
                || clientLocation.Y < bounds.Top || clientLocation.Y >= bounds.Bottom
                || !Interop.ClientToScreen(handle, ref point) || !CanClickAt(handle, point)
                || Control.MouseButtons != MouseButtons.None) { return false; }
            Point target = new Point(point.X, point.Y);
            if (!Interop.SetCursorPos(point.X, point.Y)) { return false; }
            WaitForNextScan(50);
            for (int i = 0; i < clickCount; i++)
            {
                lock (this.mouseLock)
                {
                    if (this.stopSignal.IsSet || !this.enabled
                        || (ruleId > 0 && !this.rules[ruleId - 1].enabled)
                        || !CanClickAt(handle, point) || System.Windows.Forms.Cursor.Position != target
                        || Control.MouseButtons != MouseButtons.None) { return false; }
                    if (!Interop.SendMouseButton(Constants.MOUSEEVENTF_LEFTDOWN)) { return false; }
                    this.mouseHeld = true;
                    this.heldRuleId = ruleId;
                    this.heldWindow = handle;
                    this.heldPosition = target;
                }
                try { WaitForNextScan(70); }
                finally { ReleaseMouseHold(); }
                if (i + 1 < clickCount) { WaitForNextScan(100); }
            }
            return true;
        }

        private void RandomClickIfDue(Client client)
        {
            if (this.stopSignal.IsSet || !this.enabled || !IsTargetAvailable(client)
                || Interop.GetForegroundWindow() != client.process.MainWindowHandle)
            {
                this.randomClickTimer.Restart();
                return;
            }
            if (this.randomClickTimer.ElapsedMilliseconds < 10000) { return; }
            Interop.RECT bounds;
            if (!Interop.GetClientRect(client.process.MainWindowHandle, out bounds)
                || bounds.Right <= bounds.Left || bounds.Bottom <= bounds.Top) { return; }
            int width = bounds.Right - bounds.Left, height = bounds.Bottom - bounds.Top;
            int areaWidth = Math.Max(1, width / 4), areaHeight = Math.Max(1, height / 4);
            int left = bounds.Left + (width - areaWidth) / 2;
            int top = bounds.Top + (height - areaHeight) / 2;
            int right = Math.Max(left + 1, bounds.Left + width / 2);
            Point location = new Point(this.randomClickPosition.Next(left, right),
                this.randomClickPosition.Next(top, top + areaHeight));
            if (ClickPixel(client, location, 0, 1)) { this.randomClickTimer.Restart(); }
        }

        private void ReleaseMouseHold()
        {
            lock (this.mouseLock)
            {
                if (!this.mouseHeld) { return; }
                if (!Interop.SendMouseButton(Constants.MOUSEEVENTF_LEFTUP))
                {
                    Interop.mouse_event(Constants.MOUSEEVENTF_LEFTUP, 0, 0, 0, 0);
                }
                this.mouseHeld = false;
                this.heldRuleId = 0;
            }
        }

        private static bool CanClickAt(IntPtr handle, Interop.POINT screenPoint)
        {
            return Interop.GetForegroundWindow() == handle
                && Interop.GetAncestor(Interop.WindowFromPoint(screenPoint), 2) == handle;
        }

    }
}
