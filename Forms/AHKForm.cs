using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using _4RTools.Utils;
using _4RTools.Model;
using System.Windows.Input;
using System.Web;

namespace _4RTools.Forms
{
    public partial class AHKForm : Form, IObserver
    {

        public AHKForm(Subject subject)
        {
            InitializeComponent();
            InitializeCheckAsThreeState();
            SetSkillKeyImages();
            subject.Attach(this);
        }

        public void Update(ISubject subject)
        {
            switch ((subject as Subject).Message.code)
            {
                case MessageCode.PROFILE_CHANGED:
                    RemoveHandlers();
                    FormUtils.ResetForm(this);
                    SetLegendDefaultValues();
                    InitializeCheckAsThreeState();

                    RadioButton rdAhkMode = (RadioButton)this.groupAhkConfig.Controls[ProfileSingleton.GetCurrent().AHK.ahkMode];
                    if (rdAhkMode != null) { rdAhkMode.Checked = true; };
                    this.txtSpammerDelay.Text = ProfileSingleton.GetCurrent().AHK.AhkDelay.ToString();
                    this.chkNoShift.Checked = ProfileSingleton.GetCurrent().AHK.noShift;
                    this.chkMouseFlick.Checked = ProfileSingleton.GetCurrent().AHK.mouseFlick;
                    this.DisableControlsIfSpeedBoost();

                    Dictionary<string, KeyConfig> ahkClones = new Dictionary<string, KeyConfig>(ProfileSingleton.GetCurrent().AHK.AhkEntries);

                    foreach (KeyValuePair<string, KeyConfig> config in ahkClones)
                    {
                        ToggleCheckboxByName(config.Key, config.Value.ClickActive);
                    }
                    break;
                case MessageCode.TURN_ON:
                    ProfileSingleton.GetCurrent().AHK.Start();
                    break;
                case MessageCode.TURN_OFF:
                    ProfileSingleton.GetCurrent().AHK.Stop();
                    break;
            }
        }

        private void onCheckChange(object sender, EventArgs e)
        {
            CheckBox checkbox = (CheckBox)sender;

            string keyName = checkbox.Tag as string ?? checkbox.Text;
            Key key = (Key)new KeyConverter().ConvertFromString(keyName);
            bool haveMouseClick = checkbox.CheckState == CheckState.Checked ? true : false;

            if (checkbox.CheckState == CheckState.Checked || checkbox.CheckState == CheckState.Indeterminate)
                ProfileSingleton.GetCurrent().AHK.AddAHKEntry(checkbox.Name, new KeyConfig(key, haveMouseClick));
            else
                ProfileSingleton.GetCurrent().AHK.RemoveAHKEntry(checkbox.Name);

            ProfileSingleton.SetConfiguration(ProfileSingleton.GetCurrent().AHK);
        }

        private void txtSpammerDelay_TextChanged(object sender, EventArgs e)
        {
            try
            {
                ProfileSingleton.GetCurrent().AHK.AhkDelay = Convert.ToInt16(this.txtSpammerDelay.Value);
                ProfileSingleton.SetConfiguration(ProfileSingleton.GetCurrent().AHK);
            }
            catch { }
        }

        private void ToggleCheckboxByName(string Name, bool state)
        {
            try
            {
                CheckBox checkBox = (CheckBox)this.Controls.Find(Name, true)[0];
                checkBox.CheckState = state ? CheckState.Checked : CheckState.Indeterminate;
                ProfileSingleton.SetConfiguration(ProfileSingleton.GetCurrent().AHK);
            }
            catch { }
        }

        private void RemoveHandlers()
        {
            foreach (Control c in this.Controls)
                if (c is CheckBox)
                {
                    CheckBox check = (CheckBox)c;
                    check.CheckStateChanged -= onCheckChange;
                }
        }


        private void InitializeCheckAsThreeState()
        {
            foreach (Control c in this.Controls)
                if (c is CheckBox)
                {
                    CheckBox check = (CheckBox)c;
                    if((check.Name.Split(new[] { "chk" }, StringSplitOptions.None).Length == 2)){
                        check.ThreeState = true;
                    };

                    if(check.Enabled)
                        check.CheckStateChanged += onCheckChange;
                }
        }

        private void SetLegendDefaultValues()
        {
            this.cbWithNoClick.ThreeState = true;
            this.cbWithNoClick.CheckState = System.Windows.Forms.CheckState.Indeterminate;
            this.cbWithNoClick.AutoCheck = false;
            this.cbWithClick.CheckState = System.Windows.Forms.CheckState.Checked;
            this.cbWithClick.ThreeState = true;
            this.cbWithClick.AutoCheck = false;
        }

        private void SetSkillKeyImages()
        {
            foreach (Control control in this.Controls)
            {
                CheckBox checkBox = control as CheckBox;
                if (checkBox == null || !checkBox.Name.StartsWith("chk") || string.IsNullOrEmpty(checkBox.Text))
                {
                    continue;
                }

                string path = ResolveKeyAssetPath(checkBox.Text);
                if (!File.Exists(path))
                {
                    continue;
                }

                string keyName = checkBox.Text;
                checkBox.Tag = keyName;
                checkBox.Text = string.Empty;
                checkBox.AutoSize = false;
                checkBox.Size = new Size(43, 23);
                checkBox.Padding = Padding.Empty;
                checkBox.CheckAlign = ContentAlignment.MiddleLeft;
                checkBox.Image = CreateSkillKeyIcon(path);
                checkBox.ImageAlign = ContentAlignment.MiddleRight;
                checkBox.TextImageRelation = TextImageRelation.ImageBeforeText;
                checkBox.AccessibleName = keyName;
            }
        }

        private static Bitmap CreateSkillKeyIcon(string path)
        {
            const int canvasWidth = 25;
            const int canvasHeight = 21;
            Bitmap icon = new Bitmap(canvasWidth, canvasHeight);

            using (Image source = Image.FromFile(path))
            using (Graphics graphics = Graphics.FromImage(icon))
            {
                graphics.Clear(Color.Transparent);
                graphics.CompositingQuality = System.Drawing.Drawing2D.CompositingQuality.HighQuality;
                graphics.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
                graphics.PixelOffsetMode = System.Drawing.Drawing2D.PixelOffsetMode.HighQuality;
                graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;

                int drawWidth = Math.Min(source.Width, canvasWidth);
                int drawHeight = Math.Min(source.Height, canvasHeight);
                int x = (canvasWidth - drawWidth) / 2;
                int y = (canvasHeight - drawHeight) / 2;
                graphics.DrawImage(source, new Rectangle(x, y, drawWidth, drawHeight));
            }

            return icon;
        }

        private static string ResolveKeyAssetPath(string keyName)
        {
            string fileName = "key_" + keyName.ToLowerInvariant() + ".png";
            string path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "assets", "images", "keys", fileName);
            if (File.Exists(path))
            {
                return path;
            }

            return Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "..", "..", "assets", "images", "keys", fileName);
        }

        private void RadioButton_CheckedChanged(object sender, EventArgs e)
        {
            RadioButton rb = sender as RadioButton;
            if (rb.Checked)
            {
                ProfileSingleton.GetCurrent().AHK.ahkMode = rb.Name;
                ProfileSingleton.SetConfiguration(ProfileSingleton.GetCurrent().AHK);
                this.DisableControlsIfSpeedBoost();
            }
        }

        private void chkMouseFlick_CheckedChanged(object sender, EventArgs e)
        {
            CheckBox chk = sender as CheckBox;
            ProfileSingleton.GetCurrent().AHK.mouseFlick = chk.Checked;
            ProfileSingleton.SetConfiguration(ProfileSingleton.GetCurrent().AHK);
        }

        private void chkNoShift_CheckedChanged(object sender, EventArgs e)
        {
            CheckBox chk = sender as CheckBox;
            ProfileSingleton.GetCurrent().AHK.noShift = chk.Checked;
            ProfileSingleton.SetConfiguration(ProfileSingleton.GetCurrent().AHK);
        }

        private void DisableControlsIfSpeedBoost()
        {
            if (ProfileSingleton.GetCurrent().AHK.ahkMode == AHK.SPEED_BOOST)
            {
                this.chkMouseFlick.Enabled = false;
                this.chkNoShift.Enabled = false;
            } else
            {
                this.chkMouseFlick.Enabled = true;
                this.chkNoShift.Enabled = true;
            }
        }
    }
}
