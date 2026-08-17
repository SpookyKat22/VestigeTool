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
        private const int TotalRules = 3;
        private bool loading;

        public PixelMacroForm(Subject subject)
        {
            InitializeComponent();
            subject.Attach(this);
        }

        private void InitializeComponent()
        {
            this.BackColor = Color.FromArgb(226, 241, 252);
            this.ClientSize = new Size(563, 274);
            this.FormBorderStyle = FormBorderStyle.None;
            this.Name = "PixelMacroForm";
            this.Text = "PixelMacroForm";

            AddHeader();
            for (int i = 1; i <= TotalRules; i++)
            {
                AddRuleRow(i, 34 + ((i - 1) * 38));
            }
        }

        private void AddHeader()
        {
            AddLabel("On", 14, 10, true);
            AddLabel("Color", 54, 10, true);
            AddLabel("Tol", 145, 10, true);
            AddLabel("Key", 210, 10, true);
            AddLabel("Delay", 292, 10, true);
            AddLabel("Tools", 384, 10, true);
            AddLabel("Name", 493, 10, true);
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
                Text = "#000000",
                TextAlign = HorizontalAlignment.Center
            };
            colorInput.TextChanged += onRuleChanged;

            NumericUpDown toleranceInput = NewNumber($"numPixelTolerance{id}", 138, y, 255, 48);
            toleranceInput.Value = 10;

            TextBox keyInput = new TextBox
            {
                Name = $"txtPixelKey{id}",
                Font = new Font("Segoe UI", 8.5F),
                Location = new Point(198, y),
                Size = new Size(70, 23),
                Text = Key.None.ToString(),
                TextAlign = HorizontalAlignment.Center
            };
            keyInput.KeyDown += FormUtils.OnKeyDown;
            keyInput.KeyPress += FormUtils.OnKeyPress;
            keyInput.TextChanged += onRuleChanged;

            NumericUpDown delayInput = NewNumber($"numPixelDelay{id}", 286, y, 60000, 74);
            delayInput.Increment = 50;
            delayInput.Value = 250;

            Button colorButton = NewButton($"btnPixelColor{id}", "Color", 378, y - 1, 50);
            colorButton.Tag = id;
            colorButton.Click += onColorClick;

            Button pickButton = NewButton($"btnPixelPick{id}", "Pick", 434, y - 1, 46);
            pickButton.Tag = id;
            pickButton.Click += onPickClick;

            TextBox nameInput = new TextBox
            {
                Name = $"txtPixelName{id}",
                Font = new Font("Segoe UI", 8.5F),
                Location = new Point(486, y),
                Size = new Size(70, 23),
                TextAlign = HorizontalAlignment.Center
            };
            nameInput.TextChanged += onRuleChanged;

            this.Controls.Add(enabled);
            this.Controls.Add(colorInput);
            this.Controls.Add(toleranceInput);
            this.Controls.Add(keyInput);
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
                Font = new Font("Segoe UI", 8.25F),
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
                    ProfileSingleton.GetCurrent().PixelMacro.Start();
                    break;
                case MessageCode.TURN_OFF:
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
                Find<NumericUpDown>($"numPixelTolerance{rule.id}").Value = Clamp(rule.tolerance, 0, 255);
                Find<TextBox>($"txtPixelKey{rule.id}").Text = rule.key.ToString();
                Find<NumericUpDown>($"numPixelDelay{rule.id}").Value = Clamp(rule.delay, 0, 60000);
                Find<TextBox>($"txtPixelName{rule.id}").Text = rule.name ?? "";
            }

            loading = false;
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
                rule.key = (Key)Enum.Parse(typeof(Key), Find<TextBox>($"txtPixelKey{id}").Text);
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
