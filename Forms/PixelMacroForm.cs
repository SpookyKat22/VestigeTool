using System;
using System.Drawing;
using System.Globalization;
using System.Windows.Forms;
using System.Windows.Input;
using _4RTools.Model;
using _4RTools.Utils;

namespace _4RTools.Forms
{
    public class PixelMacroForm : Form, IObserver
    {
        private const int TotalRules = 4;
        private bool loading;
        private bool sessionActive;

        public PixelMacroForm(Subject subject)
        {
            loading = true;
            InitializeComponent();
            loading = false;
            subject.Attach(this);
        }

        private void InitializeComponent()
        {
            this.BackColor = Color.FromArgb(242, 248, 252);
            this.ClientSize = new Size(563, 330);
            this.FormBorderStyle = FormBorderStyle.None;
            this.Name = "PixelMacroForm";
            this.Text = "PixelMacroForm";

            AddHeader();
            for (int i = 1; i <= TotalRules; i++)
            {
                int ruleId = i == 1 ? 4 : i == 2 ? 2 : i == 3 ? 1 : 3;
                AddRuleRow(ruleId, 67 + ((i - 1) * 29));
            }
            AddDetectionSettings();
            AddLabel("Keep the game in front. The cursor stays at the clicked target.", 20, 302, false);

            Control[] controls = new Control[this.Controls.Count];
            this.Controls.CopyTo(controls, 0);
            foreach (Control control in controls)
            {
                if (control is Panel)
                {
                    control.SendToBack();
                }
            }
        }

        private void AddDetectionSettings()
        {
            Label automaticArea = new Label
            {
                AutoSize = true,
                Font = new Font("Segoe UI Semibold", 9F, FontStyle.Bold),
                ForeColor = Color.FromArgb(39, 121, 169),
                Location = new Point(22, 190),
                Text = "Search: entire client; Evade: centered 1/4 width and height"
            };
            this.Controls.Add(automaticArea);

            AddLabel("Block", 22, 218, true);
            NumericUpDown block = NewSettingNumber("numPixelBlockSize", 66, 214, 64, 55);
            block.Minimum = 1;
            block.Maximum = 32;
            block.Value = 4;

            AddLabel("Scan ms", 139, 218, true);
            NumericUpDown scan = NewSettingNumber("numPixelScanDelay", 198, 214, 60000, 70);
            scan.Minimum = 10;
            scan.Value = 10;

            AddLabel("MACRO KEY (Flywing / Teleport)", 22, 267, true);
            TextBox followUpKey = new TextBox
            {
                Name = "txtPixelFollowUpKey",
                Font = new Font("Segoe UI", 8.5F),
                BackColor = Color.White,
                ForeColor = Color.FromArgb(30, 80, 118),
                Location = new Point(215, 262),
                Size = new Size(110, 25),
                Text = Key.None.ToString(),
                TextAlign = HorizontalAlignment.Center
            };
            followUpKey.KeyDown += FormUtils.OnKeyDown;
            followUpKey.KeyPress += FormUtils.OnKeyPress;
            followUpKey.TextChanged += onSettingsChanged;
            this.Controls.Add(followUpKey);

            CheckBox macroEnabled = new CheckBox
            {
                Name = "chkPixelMacroEnabled",
                AutoSize = true,
                Font = new Font("Segoe UI Semibold", 8.5F, FontStyle.Bold),
                ForeColor = Color.FromArgb(30, 80, 118),
                Location = new Point(365, 264),
                Checked = false,
                Text = "Pixel Macro ON"
            };
            macroEnabled.CheckedChanged += onSettingsChanged;
            this.Controls.Add(macroEnabled);
        }

        private NumericUpDown NewSettingNumber(string name, int x, int y, int max, int width)
        {
            NumericUpDown input = NewNumber(name, x, y, max, width);
            input.ValueChanged -= onRuleChanged;
            input.ValueChanged += onSettingsChanged;
            this.Controls.Add(input);
            return input;
        }

        private void AddHeader()
        {
            AddCard(10, 38, 543, 144);
            AddCard(10, 187, 543, 60);
            AddCard(10, 254, 543, 40);

            Label title = new Label
            {
                AutoSize = true,
                Font = new Font("Segoe UI Semibold", 12F, FontStyle.Bold),
                ForeColor = Color.FromArgb(30, 80, 118),
                Location = new Point(12, 8),
                Text = "Pixel Macro"
            };
            this.Controls.Add(title);

            AddLabel("ON", 22, 47, true);
            AddLabel("TARGET COLOR", 54, 47, true);
            AddLabel("TOL", 145, 47, true);
            AddLabel("CLICK DELAY", 210, 47, true);
            AddLabel("COLOR TOOLS", 302, 47, true);
            AddLabel("RULE NAME", 411, 47, true);
        }

        private void AddCard(int x, int y, int width, int height)
        {
            Panel card = new Panel
            {
                BackColor = Color.White,
                BorderStyle = BorderStyle.FixedSingle,
                Location = new Point(x, y),
                Size = new Size(width, height)
            };
            this.Controls.Add(card);
            card.SendToBack();
        }

        private void AddRuleRow(int id, int y)
        {
            CheckBox enabled = new CheckBox
            {
                Name = $"chkPixel{id}",
                Location = new Point(18, y + 5),
                Size = new Size(15, 14)
            };
            enabled.CheckedChanged += onRuleChanged;

            TextBox colorInput = new TextBox
            {
                Name = $"txtPixelColor{id}",
                Font = new Font("Segoe UI", 8.5F),
                Location = new Point(50, y),
                Size = new Size(74, 23),
                Text = id == 1 ? "#394ACE" : id == 2 ? "#FF0000" : id == 3 ? "#537559" : "#FFF700",
                TextAlign = HorizontalAlignment.Center
            };
            colorInput.TextChanged += onRuleChanged;

            NumericUpDown toleranceInput = NewNumber($"numPixelTolerance{id}", 138, y, 5, 48);
            toleranceInput.Value = 5;

            NumericUpDown delayInput = NewNumber($"numPixelDelay{id}", 198, y, 60000, 74);
            delayInput.Increment = 10;
            delayInput.Value = 50;
            if (id == 4) { delayInput.Value = 500; delayInput.Enabled = false; }

            Button colorButton = NewButton($"btnPixelColor{id}", "Color", 290, y - 1, 50);
            colorButton.Tag = id;
            colorButton.Click += onColorClick;

            Button pickButton = NewButton($"btnPixelPick{id}", "Pick", 346, y - 1, 46);
            pickButton.Tag = id;
            pickButton.Click += onPickClick;

            TextBox nameInput = new TextBox
            {
                Name = $"txtPixelName{id}",
                Font = new Font("Segoe UI", 8.5F),
                Location = new Point(398, y),
                Size = new Size(158, 23),
                Text = id == 1 ? "Attack Pixel" : id == 2 ? "Evade Pixel" : id == 3 ? "8-second Pixel" : "Map Pixel",
                ReadOnly = true,
                TabStop = false,
                BackColor = Color.FromArgb(242, 248, 252),
                TextAlign = HorizontalAlignment.Center
            };

            this.Controls.Add(enabled);
            this.Controls.Add(colorInput);
            this.Controls.Add(toleranceInput);
            this.Controls.Add(delayInput);
            this.Controls.Add(colorButton);
            this.Controls.Add(pickButton);
            this.Controls.Add(nameInput);
        }

        private NumericUpDown NewNumber(string name, int x, int y, int max, int width)
        {
            NumericUpDown input = new NumericUpDown
            {
                Name = name,
                Font = new Font("Segoe UI", 8.5F),
                Location = new Point(x, y),
                Maximum = max,
                Size = new Size(width, 23)
            };
            input.ValueChanged += onRuleChanged;
            return input;
        }

        private static Button NewButton(string name, string text, int x, int y, int width)
        {
            return new Button
            {
                BackColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                FlatAppearance = { BorderColor = Color.FromArgb(179, 205, 222) },
                Font = new Font("Segoe UI Semibold", 8.25F, FontStyle.Bold),
                ForeColor = Color.FromArgb(30, 80, 118),
                Location = new Point(x, y),
                Name = name,
                Size = new Size(width, 25),
                Text = text,
                UseVisualStyleBackColor = false
            };
        }

        private void AddLabel(string text, int x, int y, bool bold)
        {
            Label label = new Label
            {
                AutoSize = true,
                Font = new Font("Segoe UI", 8.5F, bold ? FontStyle.Bold : FontStyle.Regular),
                Location = new Point(x, y),
                Text = text
            };
            this.Controls.Add(label);
        }

        public void Update(ISubject subject)
        {
            switch ((subject as Subject).Message.code)
            {
                case MessageCode.PROFILE_CHANGED:
                    updateUi();
                    break;
                case MessageCode.TURN_ON:
                    sessionActive = true;
                    ProfileSingleton.GetCurrent().PixelMacro.Start();
                    break;
                case MessageCode.TURN_OFF:
                    sessionActive = false;
                    ProfileSingleton.GetCurrent().PixelMacro.Stop();
                    break;
            }
        }

        private void updateUi()
        {
            loading = true;
            PixelMacro pixelMacro = ProfileSingleton.GetCurrent().PixelMacro;
            pixelMacro.EnsureRules();

            foreach (PixelMacroRule rule in pixelMacro.rules)
            {
                if (rule.id > TotalRules) { continue; }
                Find<CheckBox>($"chkPixel{rule.id}").Checked = rule.enabled;
                Find<TextBox>($"txtPixelColor{rule.id}").Text = ColorToHex(rule);
                rule.tolerance = Math.Min(rule.tolerance, 5);
                Find<NumericUpDown>($"numPixelTolerance{rule.id}").Value = Clamp(rule.tolerance, 0, 5);
                rule.delay = rule.id == 4 ? 500 : Math.Min(rule.delay, 50);
                Find<NumericUpDown>($"numPixelDelay{rule.id}").Value = Clamp(rule.delay, 0, 60000);
                Find<TextBox>($"txtPixelName{rule.id}").Text = rule.name ?? "";
            }

            pixelMacro.useSearchArea = false;
            Find<NumericUpDown>("numPixelBlockSize").Value = Clamp(pixelMacro.blockSize, 1, 32);
            pixelMacro.scanDelay = Math.Min(pixelMacro.scanDelay, 10);
            Find<NumericUpDown>("numPixelScanDelay").Value = Clamp(pixelMacro.scanDelay, 10, 60000);
            Find<TextBox>("txtPixelFollowUpKey").Text = pixelMacro.followUpKey.ToString();
            Find<CheckBox>("chkPixelMacroEnabled").Checked = pixelMacro.enabled;

            loading = false;
        }

        private void onSettingsChanged(object sender, EventArgs e)
        {
            if (loading) { return; }

            PixelMacro pixelMacro = ProfileSingleton.GetCurrent().PixelMacro;
            pixelMacro.enabled = Find<CheckBox>("chkPixelMacroEnabled").Checked;
            pixelMacro.useSearchArea = false;
            pixelMacro.blockSize = Convert.ToInt32(Find<NumericUpDown>("numPixelBlockSize").Value);
            pixelMacro.scanDelay = Convert.ToInt32(Find<NumericUpDown>("numPixelScanDelay").Value);
            Key followUpKey;
            if (Enum.TryParse(Find<TextBox>("txtPixelFollowUpKey").Text, out followUpKey))
            {
                pixelMacro.followUpKey = followUpKey;
            }
            ProfileSingleton.SetConfiguration(pixelMacro);

            Control changedControl = sender as Control;
            if (changedControl != null && changedControl.Name == "chkPixelMacroEnabled")
            {
                if (pixelMacro.enabled && sessionActive)
                {
                    pixelMacro.Start();
                }
                else
                {
                    pixelMacro.Stop();
                }
            }
        }

        private void onPickClick(object sender, EventArgs e)
        {
            int id = Convert.ToInt32(((Button)sender).Tag);
            using (ScreenColorPickerDialog picker = new ScreenColorPickerDialog())
            {
                if (picker.ShowDialog() == DialogResult.OK)
                {
                    Find<TextBox>($"txtPixelColor{id}").Text = ColorToHex(picker.SelectedColor);
                    SaveRule(id, true);
                }
            }
        }

        private void onColorClick(object sender, EventArgs e)
        {
            int id = Convert.ToInt32(((Button)sender).Tag);
            using (ColorDialog colorDialog = new ColorDialog())
            {
                try
                {
                    colorDialog.Color = ParseColor(Find<TextBox>($"txtPixelColor{id}").Text);
                }
                catch
                {
                    colorDialog.Color = Color.Black;
                }

                if (colorDialog.ShowDialog() == DialogResult.OK)
                {
                    Find<TextBox>($"txtPixelColor{id}").Text = ColorToHex(colorDialog.Color);
                    SaveRule(id, true);
                }
            }
        }

        private void onRuleChanged(object sender, EventArgs e)
        {
            if (loading) { return; }
            Control control = (Control)sender;
            int id = ExtractId(control.Name);
            SaveRule(id, control.Name.StartsWith("txtPixelColor"));
        }

        private void SaveRule(int id, bool markColorConfigured = false)
        {
            try
            {
                PixelMacro pixelMacro = ProfileSingleton.GetCurrent().PixelMacro;
                pixelMacro.EnsureRules();
                PixelMacroRule rule = pixelMacro.rules[id - 1];
                Color color = ParseColor(Find<TextBox>($"txtPixelColor{id}").Text);

                rule.enabled = Find<CheckBox>($"chkPixel{id}").Checked;
                rule.red = color.R;
                rule.green = color.G;
                rule.blue = color.B;
                rule.hasColor = rule.hasColor || markColorConfigured;
                rule.tolerance = Convert.ToInt32(Find<NumericUpDown>($"numPixelTolerance{id}").Value);
                rule.delay = Convert.ToInt32(Find<NumericUpDown>($"numPixelDelay{id}").Value);
                rule.name = Find<TextBox>($"txtPixelName{id}").Text;

                ProfileSingleton.SetConfiguration(pixelMacro);
            }
            catch { }
        }

        private T Find<T>(string name) where T : Control
        {
            return (T)this.Controls.Find(name, true)[0];
        }

        private static int ExtractId(string name)
        {
            string digits = string.Empty;
            foreach (char c in name)
            {
                if (char.IsDigit(c)) { digits += c; }
            }
            return Int32.Parse(digits);
        }

        private static decimal Clamp(int value, int min, int max)
        {
            return Math.Max(min, Math.Min(max, value));
        }

        private static string ColorToHex(PixelMacroRule rule)
        {
            return $"#{rule.red:X2}{rule.green:X2}{rule.blue:X2}";
        }

        private static string ColorToHex(Color color)
        {
            return $"#{color.R:X2}{color.G:X2}{color.B:X2}";
        }

        private static Color ParseColor(string value)
        {
            string hex = value.Trim().TrimStart('#');
            int rgb = Int32.Parse(hex, NumberStyles.HexNumber);
            return Color.FromArgb((rgb >> 16) & 255, (rgb >> 8) & 255, rgb & 255);
        }

        private static Color ReadScreenPixel(int x, int y)
        {
            using (Bitmap bitmap = new Bitmap(1, 1))
            using (Graphics graphics = Graphics.FromImage(bitmap))
            {
                graphics.CopyFromScreen(x, y, 0, 0, new Size(1, 1));
                return bitmap.GetPixel(0, 0);
            }
        }

        private class ScreenColorPickerDialog : Form
        {
            private readonly Timer timer = new Timer();
            private readonly Label positionLabel = new Label();
            private readonly Label colorLabel = new Label();
            private readonly Panel previewPanel = new Panel();
            private readonly DateTime openedAt = DateTime.Now;

            public Color SelectedColor { get; private set; }

            public ScreenColorPickerDialog()
            {
                this.BackColor = Color.White;
                this.ClientSize = new Size(230, 92);
                this.FormBorderStyle = FormBorderStyle.FixedToolWindow;
                this.KeyPreview = true;
                this.StartPosition = FormStartPosition.CenterParent;
                this.Text = "Pick Pixel Color";
                this.TopMost = true;

                Label hintLabel = new Label
                {
                    AutoSize = true,
                    Location = new Point(10, 8),
                    Text = "Move mouse, then click or press Enter."
                };

                this.positionLabel.AutoSize = true;
                this.positionLabel.Location = new Point(10, 33);

                this.colorLabel.AutoSize = true;
                this.colorLabel.Location = new Point(10, 57);

                this.previewPanel.BorderStyle = BorderStyle.FixedSingle;
                this.previewPanel.Location = new Point(172, 30);
                this.previewPanel.Size = new Size(42, 42);

                this.Controls.Add(hintLabel);
                this.Controls.Add(this.positionLabel);
                this.Controls.Add(this.colorLabel);
                this.Controls.Add(this.previewPanel);

                this.timer.Interval = 40;
                this.timer.Tick += onTimerTick;
                this.timer.Start();
            }

            protected override void OnKeyDown(System.Windows.Forms.KeyEventArgs e)
            {
                if (e.KeyCode == Keys.Enter || e.KeyCode == Keys.Space)
                {
                    PickCurrentPixel();
                    e.Handled = true;
                }
                else if (e.KeyCode == Keys.Escape)
                {
                    this.DialogResult = DialogResult.Cancel;
                    this.Close();
                    e.Handled = true;
                }

                base.OnKeyDown(e);
            }

            protected override void OnFormClosed(FormClosedEventArgs e)
            {
                this.timer.Stop();
                this.timer.Dispose();
                base.OnFormClosed(e);
            }

            private void onTimerTick(object sender, EventArgs e)
            {
                Point cursor = System.Windows.Forms.Cursor.Position;
                Color color = ReadScreenPixel(cursor.X, cursor.Y);
                this.SelectedColor = color;
                this.positionLabel.Text = $"X: {cursor.X}  Y: {cursor.Y}";
                this.colorLabel.Text = $"Color: {ColorToHex(color)}";
                this.previewPanel.BackColor = color;

                if ((DateTime.Now - this.openedAt).TotalMilliseconds > 350 && Control.MouseButtons == MouseButtons.Left)
                {
                    PickCurrentPixel();
                }
            }

            private void PickCurrentPixel()
            {
                Point cursor = System.Windows.Forms.Cursor.Position;
                this.SelectedColor = ReadScreenPixel(cursor.X, cursor.Y);
                this.DialogResult = DialogResult.OK;
                this.Close();
            }
        }
    }
}
