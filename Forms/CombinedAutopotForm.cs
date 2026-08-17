using System;
using System.Drawing;
using System.Windows.Forms;
using System.Windows.Input;
using _4RTools.Model;
using _4RTools.Utils;

namespace _4RTools.Forms
{
    public class CombinedAutopotForm : Form, IObserver
    {
        private Autopot autopot;
        private Autopot yggAutopot;
        private bool loading;

        private TextBox txtHpKey;
        private TextBox txtSpKey;
        private TextBox txtYggHpKey;
        private TextBox txtYggSpKey;
        private NumericUpDown txtHpPct;
        private NumericUpDown txtSpPct;
        private NumericUpDown txtYggHpPct;
        private NumericUpDown txtYggSpPct;
        private TextBox txtDelay;

        public CombinedAutopotForm(Subject subject)
        {
            InitializeComponent();
            subject.Attach(this);
        }

        private void InitializeComponent()
        {
            this.BackColor = Color.FromArgb(232, 244, 252);
            this.ClientSize = new Size(300, 139);
            this.FormBorderStyle = FormBorderStyle.None;
            this.Name = "CombinedAutopotForm";
            this.Text = "CombinedAutopotForm";

            AddHeader("Autopot HP", 53);
            AddHeader("Autopot SP", 191);
            AddPercentHeader("HP%", 119, Color.FromArgb(210, 42, 42));
            AddPercentHeader("SP%", 257, Color.FromArgb(26, 92, 190));

            AddRow(false, true, 18, 35, Resources._4RTools.ETCResource.HP);
            AddRow(true, true, 18, 63, Resources._4RTools.ETCResource.Yggdrasil);
            AddRow(false, false, 156, 35, Resources._4RTools.ETCResource.SP);
            AddRow(true, false, 156, 63, Resources._4RTools.ETCResource.Yggdrasil);

            AddDelay(121, 97);
        }

        private void AddHeader(string text, int x)
        {
            Label header = new Label
            {
                AutoSize = true,
                Font = new Font("Segoe UI", 8.5F, FontStyle.Bold),
                Location = new Point(x, 8),
                ForeColor = Color.FromArgb(34, 48, 58),
                Text = text
            };

            this.Controls.Add(header);
        }

        private void AddPercentHeader(string text, int x, Color color)
        {
            Label header = new Label
            {
                AutoSize = true,
                Font = new Font("Segoe UI", 8F, FontStyle.Bold),
                Location = new Point(x, 21),
                ForeColor = color,
                Text = text
            };

            this.Controls.Add(header);
        }

        private void AddRow(bool isYgg, bool isHp, int x, int y, Image icon)
        {
            PictureBox pictureBox = new PictureBox
            {
                Image = icon,
                Location = new Point(x, y),
                Size = new Size(24, 24),
                SizeMode = PictureBoxSizeMode.CenterImage
            };

            Label label = new Label
            {
                AutoSize = true,
                Font = new Font("Segoe UI", 8.5F),
                Location = new Point(x + 30, y + 5),
                ForeColor = Color.FromArgb(34, 48, 58),
                Text = isYgg ? "Ygg" : "Pot"
            };

            TextBox keyBox = new TextBox
            {
                Font = new Font("Segoe UI", 8.5F),
                Location = new Point(x + 54, y + 1),
                Size = new Size(44, 23),
                TextAlign = HorizontalAlignment.Center
            };
            keyBox.KeyDown += FormUtils.OnKeyDown;
            keyBox.KeyPress += FormUtils.OnKeyPress;
            keyBox.TextChanged += onKeyTextChanged;

            NumericUpDown percentBox = new NumericUpDown
            {
                Font = new Font("Segoe UI", 8.5F),
                Location = new Point(x + 101, y + 1),
                Maximum = 100,
                Size = new Size(40, 23)
            };
            percentBox.ValueChanged += onPercentChanged;

            if (isYgg)
            {
                if (isHp)
                {
                    txtYggHpKey = keyBox;
                    txtYggHpPct = percentBox;
                }
                else
                {
                    txtYggSpKey = keyBox;
                    txtYggSpPct = percentBox;
                }
            }
            else
            {
                if (isHp)
                {
                    txtHpKey = keyBox;
                    txtHpPct = percentBox;
                }
                else
                {
                    txtSpKey = keyBox;
                    txtSpPct = percentBox;
                }
            }

            this.Controls.Add(pictureBox);
            this.Controls.Add(label);
            this.Controls.Add(keyBox);
            this.Controls.Add(percentBox);
        }

        private void AddDelay(int x, int y)
        {
            Label label = new Label
            {
                AutoSize = true,
                Font = new Font("Segoe UI", 8.5F),
                Location = new Point(x - 41, y + 5),
                ForeColor = Color.FromArgb(34, 48, 58),
                Text = "Delay"
            };

            TextBox delayBox = new TextBox
            {
                Font = new Font("Segoe UI", 8.5F),
                Location = new Point(x, y + 1),
                Size = new Size(56, 23),
                TextAlign = HorizontalAlignment.Center
            };
            delayBox.TextChanged += onDelayChanged;

            Label unit = new Label
            {
                AutoSize = true,
                Font = new Font("Segoe UI", 8.5F),
                Location = new Point(x + 62, y + 5),
                ForeColor = Color.FromArgb(34, 48, 58),
                Text = "ms"
            };

            txtDelay = delayBox;

            this.Controls.Add(label);
            this.Controls.Add(delayBox);
            this.Controls.Add(unit);
        }

        public void Update(ISubject subject)
        {
            switch ((subject as Subject).Message.code)
            {
                case MessageCode.PROFILE_CHANGED:
                    this.autopot = ProfileSingleton.GetCurrent().Autopot;
                    this.yggAutopot = ProfileSingleton.GetCurrent().AutopotYgg;
                    InitializeApplicationForm();
                    break;
                case MessageCode.TURN_OFF:
                    this.autopot?.Stop();
                    this.yggAutopot?.Stop();
                    break;
                case MessageCode.TURN_ON:
                    this.autopot?.Start();
                    this.yggAutopot?.Start();
                    break;
            }
        }

        private void InitializeApplicationForm()
        {
            loading = true;

            txtHpKey.Text = this.autopot.hpKey.ToString();
            txtSpKey.Text = this.autopot.spKey.ToString();
            txtHpPct.Value = ClampPercent(this.autopot.hpPercent);
            txtSpPct.Value = ClampPercent(this.autopot.spPercent);
            txtDelay.Text = this.autopot.delay.ToString();

            txtYggHpKey.Text = this.yggAutopot.hpKey.ToString();
            txtYggSpKey.Text = this.yggAutopot.spKey.ToString();
            txtYggHpPct.Value = ClampPercent(this.yggAutopot.hpPercent);
            txtYggSpPct.Value = ClampPercent(this.yggAutopot.spPercent);

            loading = false;
        }

        private static decimal ClampPercent(int value)
        {
            return Math.Max(0, Math.Min(100, value));
        }

        private void onKeyTextChanged(object sender, EventArgs e)
        {
            if (loading) { return; }

            try
            {
                TextBox textBox = (TextBox)sender;
                Key key = (Key)Enum.Parse(typeof(Key), textBox.Text);
                if (textBox == txtHpKey) { this.autopot.hpKey = key; ProfileSingleton.SetConfiguration(this.autopot); }
                if (textBox == txtSpKey) { this.autopot.spKey = key; ProfileSingleton.SetConfiguration(this.autopot); }
                if (textBox == txtYggHpKey) { this.yggAutopot.hpKey = key; ProfileSingleton.SetConfiguration(this.yggAutopot); }
                if (textBox == txtYggSpKey) { this.yggAutopot.spKey = key; ProfileSingleton.SetConfiguration(this.yggAutopot); }
            }
            catch { }
        }

        private void onPercentChanged(object sender, EventArgs e)
        {
            if (loading) { return; }

            try
            {
                NumericUpDown numericUpDown = (NumericUpDown)sender;
                int value = Convert.ToInt32(numericUpDown.Value);
                if (numericUpDown == txtHpPct) { this.autopot.hpPercent = value; ProfileSingleton.SetConfiguration(this.autopot); }
                if (numericUpDown == txtSpPct) { this.autopot.spPercent = value; ProfileSingleton.SetConfiguration(this.autopot); }
                if (numericUpDown == txtYggHpPct) { this.yggAutopot.hpPercent = value; ProfileSingleton.SetConfiguration(this.yggAutopot); }
                if (numericUpDown == txtYggSpPct) { this.yggAutopot.spPercent = value; ProfileSingleton.SetConfiguration(this.yggAutopot); }
            }
            catch { }
        }

        private void onDelayChanged(object sender, EventArgs e)
        {
            if (loading) { return; }

            try
            {
                TextBox textBox = (TextBox)sender;
                int value = Int16.Parse(textBox.Text);
                if (textBox == txtDelay)
                {
                    this.autopot.delay = value;
                    this.yggAutopot.delay = value;
                    ProfileSingleton.SetConfiguration(this.autopot);
                    ProfileSingleton.SetConfiguration(this.yggAutopot);
                }
            }
            catch { }
        }
    }
}
