using System;
using System.Collections.Generic;
using System.Drawing;
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
        public int tolerance { get; set; } = 10;
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

        private _4RThread thread;
        private const int MinimumMatchSize = 4;
        public List<PixelMacroRule> rules { get; set; } = new List<PixelMacroRule>();

        public PixelMacro()
        {
            for (int i = 1; i <= 3; i++)
            {
                this.rules.Add(new PixelMacroRule(i));
            }
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
            Client roClient = ClientSingleton.GetClient();
            if (roClient != null)
            {
                EnsureRules();
                this.thread = new _4RThread((_) => PixelMacroExecutionThread(roClient));
                _4RThread.Start(this.thread);
            }
        }

        public void Stop()
        {
            _4RThread.Stop(this.thread);
        }

        public void EnsureRules()
        {
            for (int i = this.rules.Count + 1; i <= 3; i++)
            {
                this.rules.Add(new PixelMacroRule(i));
            }

            foreach (PixelMacroRule rule in this.rules)
            {
                if (!rule.hasColor && (rule.red != 0 || rule.green != 0 || rule.blue != 0))
                {
                    rule.hasColor = true;
                }
            }
        }

        private int PixelMacroExecutionThread(Client roClient)
        {
            if (!roClient.IsGameActive())
            {
                Thread.Sleep(30);
                return 0;
            }

            foreach (PixelMacroRule rule in this.rules)
            {
                if (!rule.enabled || !rule.hasColor)
                {
                    continue;
                }

                Point matchScreenLocation;
                Point matchClientLocation;
                if (TryFindMatchingPixel(roClient, rule, out matchScreenLocation, out matchClientLocation))
                {
                    if (!roClient.IsGameActive()) { continue; }
                    ClickPixel(roClient, matchScreenLocation, matchClientLocation);

                    if (rule.key != Key.None)
                    {
                        Keys key = (Keys)Enum.Parse(typeof(Keys), rule.key.ToString());
                        Interop.PostMessage(roClient.process.MainWindowHandle, Constants.WM_KEYDOWN_MSG_ID, key, 0);
                        Interop.PostMessage(roClient.process.MainWindowHandle, Constants.WM_KEYUP_MSG_ID, key, 0);
                    }

                    Thread.Sleep(Math.Max(1, rule.delay));
                }
            }

            Thread.Sleep(30);
            return 0;
        }

        private static bool TryFindMatchingPixel(Client roClient, PixelMacroRule rule, out Point matchScreenLocation, out Point matchClientLocation)
        {
            Rectangle bounds = GetClientScreenBounds(roClient);
            if (bounds.Width <= 0 || bounds.Height <= 0)
            {
                matchScreenLocation = Point.Empty;
                matchClientLocation = Point.Empty;
                return false;
            }

            using (Bitmap bitmap = CaptureClientArea(bounds))
            {
                for (int y = 0; y <= bitmap.Height - MinimumMatchSize; y++)
                {
                    for (int x = 0; x <= bitmap.Width - MinimumMatchSize; x++)
                    {
                        if (HasMinimumColorBlock(bitmap, x, y, rule))
                        {
                            int centerOffset = MinimumMatchSize / 2;
                            matchScreenLocation = new Point(bounds.Left + x + centerOffset, bounds.Top + y + centerOffset);
                            matchClientLocation = new Point(x + centerOffset, y + centerOffset);
                            return true;
                        }
                    }
                }
            }

            matchScreenLocation = Point.Empty;
            matchClientLocation = Point.Empty;
            return false;
        }

        private static Bitmap CaptureClientArea(Rectangle bounds)
        {
            Bitmap bitmap = new Bitmap(bounds.Width, bounds.Height);
            using (Graphics graphics = Graphics.FromImage(bitmap))
            {
                graphics.CopyFromScreen(bounds.Left, bounds.Top, 0, 0, bounds.Size);
            }

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

        private static bool HasMinimumColorBlock(Bitmap bitmap, int startX, int startY, PixelMacroRule rule)
        {
            for (int y = 0; y < MinimumMatchSize; y++)
            {
                for (int x = 0; x < MinimumMatchSize; x++)
                {
                    if (!ColorMatches(bitmap.GetPixel(startX + x, startY + y), rule))
                    {
                        return false;
                    }
                }
            }

            return true;
        }

        private static void ClickPixel(Client roClient, Point screenLocation, Point clientLocation)
        {
            if (!roClient.IsGameActive())
            {
                return;
            }

            if (!Interop.SetCursorPos(screenLocation.X, screenLocation.Y))
            {
                return;
            }

            Thread.Sleep(15);
            if (!roClient.IsGameActive()) { return; }
            Interop.SendLeftMouseClick();
            if (!roClient.IsGameActive()) { return; }
            SendClientClick(roClient, clientLocation);
            Thread.Sleep(15);
        }

        private static void SendClientClick(Client roClient, Point clientLocation)
        {
            int lParam = MakeLParam(clientLocation.X, clientLocation.Y);
            Interop.PostMessage(roClient.process.MainWindowHandle, Constants.WM_MOUSEMOVE, 0, lParam);
            Thread.Sleep(10);
            Interop.PostMessage(roClient.process.MainWindowHandle, Constants.WM_LBUTTONDOWN, Constants.MK_LBUTTON, lParam);
            Thread.Sleep(30);
            Interop.PostMessage(roClient.process.MainWindowHandle, Constants.WM_LBUTTONUP, 0, lParam);
        }

        private static int MakeLParam(int lowWord, int highWord)
        {
            return (highWord << 16) | (lowWord & 0xffff);
        }
    }
}
