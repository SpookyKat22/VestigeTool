using System;
using System.Windows.Forms;
using System.Windows.Input;
using System.Collections.Generic;
using _4RTools.Model;
using _4RTools.Utils;
using System.Text.RegularExpressions;
using System.Diagnostics;
using System.Xml.Linq;

namespace _4RTools.Forms
{
    public partial class MacroSwitchForm : Form, IObserver
    {
        public static int TOTAL_MACRO_LANES = 5;
        public MacroSwitchForm(Subject subject)
        {
            subject.Attach(this);
            InitializeComponent();
            RestoreSwitchTriggerControls();
            configureMacroLanes();
        }

        public void Update(ISubject subject)
        {
            switch ((subject as Subject).Message.code)
            {
                case MessageCode.PROFILE_CHANGED:
                    updateUi();
                    break;
                case MessageCode.TURN_ON:
                    ProfileSingleton.GetCurrent().MacroSwitch.Start();
                    break;
                case MessageCode.TURN_OFF:
                    ProfileSingleton.GetCurrent().MacroSwitch.Stop();
                    break;
            }
        }

        private void UpdatePanelData(int id)
        {
            try
            {
                GroupBox group = (GroupBox)this.Controls.Find("chainGroup" + id, true)[0];
                ChainConfig chainConfig = new ChainConfig(ProfileSingleton.GetCurrent().MacroSwitch.chainConfigs[id - 1]);
                FormUtils.ResetForm(group);

                Control[] triggerControls = group.Controls.Find($"triggerMac{id}", true);
                if (triggerControls.Length > 0)
                {
                    TextBox triggerBox = (TextBox)triggerControls[0];
                    triggerBox.Text = chainConfig.trigger.ToString();
                }

                List<string> names = new List<string>(chainConfig.macroEntries.Keys);
                foreach (string cbName in names)
                {
                    Control[] controls = group.Controls.Find(cbName, true); // Keys
                    if (controls.Length > 0)
                    {
                        TextBox textBox = (TextBox)controls[0];
                        textBox.Text = chainConfig.macroEntries[cbName].key.ToString();
                    }

                    Control[] d = group.Controls.Find($"{cbName}delay", true); // Delays
                    if (d.Length > 0)
                    {
                        NumericUpDown delayInput = (NumericUpDown)d[0];
                        delayInput.Value = chainConfig.macroEntries[cbName].delay;
                    }

                    Control[] c = group.Controls.Find($"{cbName}click", true); // Clicks
                    if (d.Length > 0)
                    {
                        CheckBox checkInput = (CheckBox)c[0];
                        checkInput.Checked = chainConfig.macroEntries[cbName].hasClick;
                    }


                }
            }
            catch { };
        }

        private void onTextChange(object sender, EventArgs e)
        {
            TextBox textBox = (TextBox)sender;
            int chainID = Int16.Parse(textBox.Parent.Name.Split(new[] { "chainGroup" }, StringSplitOptions.None)[1]);
            GroupBox group = (GroupBox)this.Controls.Find("chainGroup" + chainID, true)[0];
            ChainConfig chainConfig = ProfileSingleton.GetCurrent().MacroSwitch.chainConfigs.Find(config => config.id == chainID);

            Key key = (Key)Enum.Parse(typeof(Key), textBox.Text.ToString());
            if (textBox.Name == $"triggerMac{chainID}")
            {
                chainConfig.trigger = key;
                ProfileSingleton.SetConfiguration(ProfileSingleton.GetCurrent().MacroSwitch);
                return;
            }

            NumericUpDown delayInput = (NumericUpDown)group.Controls.Find($"{textBox.Name}delay", true)[0];
            chainConfig.macroEntries[textBox.Name] = new MacroKey(key, decimal.ToInt16(delayInput.Value));

            ProfileSingleton.SetConfiguration(ProfileSingleton.GetCurrent().MacroSwitch);
        }

        private void onReset(object sender, EventArgs e)
        {
            Button resetButton = (Button)sender;
            int chainID = Int16.Parse(resetButton.Name.Split(new[] { "btnResetMac" }, StringSplitOptions.None)[1]);
            ProfileSingleton.GetCurrent().MacroSwitch.ResetMacro(chainID);
            ProfileSingleton.SetConfiguration(ProfileSingleton.GetCurrent().MacroSwitch);
            UpdatePanelData(chainID);
        }


        private void onDelayChange(object sender, EventArgs e)
        {
            NumericUpDown delayInput = (NumericUpDown)sender;
            int chainID = Int16.Parse(delayInput.Parent.Name.Split(new[] { "chainGroup" }, StringSplitOptions.None)[1]);
            ChainConfig chainConfig = ProfileSingleton.GetCurrent().MacroSwitch.chainConfigs.Find(config => config.id == chainID);

            String cbName = delayInput.Name.Split(new[] { "delay" }, StringSplitOptions.None)[0];
            chainConfig.macroEntries[cbName].delay = decimal.ToInt16(delayInput.Value);

            ProfileSingleton.SetConfiguration(ProfileSingleton.GetCurrent().MacroSwitch);
        }

        private void onCheckClickChange(object sender, EventArgs e)
        {
            CheckBox checkInput = (CheckBox)sender;
            int chainID = Int16.Parse(checkInput.Parent.Name.Split(new[] { "chainGroup" }, StringSplitOptions.None)[1]);
            ChainConfig chainConfig = ProfileSingleton.GetCurrent().MacroSwitch.chainConfigs.Find(config => config.id == chainID);

            String cbName = checkInput.Name.Split(new[] { "click" }, StringSplitOptions.None)[0];
            chainConfig.macroEntries[cbName].hasClick = checkInput.Checked;
            ProfileSingleton.SetConfiguration(ProfileSingleton.GetCurrent().MacroSwitch);
        }

        private void updateUi()
        {
            for (int i = 1; i <= TOTAL_MACRO_LANES; i++)
            {
                UpdatePanelData(i);
            }
        }

        private void configureMacroLanes()
        {
            for (int i = 1; i <= TOTAL_MACRO_LANES; i++)
            {
                initializeLane(i);
            }
        }

        private void RestoreSwitchTriggerControls()
        {
            for (int i = 1; i <= TOTAL_MACRO_LANES; i++)
            {
                GroupBox group = (GroupBox)this.Controls.Find("chainGroup" + i, true)[0];
                group.Width = 625;
                ShiftChainControls(group);
                AddTriggerBox(group, i);
                AddResetButton(group, i);
            }

            this.ClientSize = new System.Drawing.Size(632, this.ClientSize.Height);
        }

        private void ShiftChainControls(GroupBox group)
        {
            foreach (Control control in group.Controls)
            {
                if (control is Label label)
                {
                    if (label.Text == "Keys:")
                    {
                        label.Location = new System.Drawing.Point(78, 18);
                    }
                    else if (label.Text == "Delays(ms):")
                    {
                        label.Location = new System.Drawing.Point(12, 51);
                    }
                    else if (label.Text == "Clicks:")
                    {
                        label.Location = new System.Drawing.Point(28, 78);
                    }

                    continue;
                }

                control.Left += 65;
            }
        }

        private void AddTriggerBox(GroupBox group, int id)
        {
            TextBox triggerBox = new TextBox
            {
                Font = this.Font,
                Location = new System.Drawing.Point(11, 28),
                Name = "triggerMac" + id,
                Size = new System.Drawing.Size(54, 20),
                Text = "None",
                TextAlign = HorizontalAlignment.Center
            };

            Label arrow = new Label
            {
                AutoSize = false,
                ForeColor = System.Drawing.Color.FromArgb(125, 135, 142),
                Location = new System.Drawing.Point(68, 29),
                Name = "triggerArrowMac" + id,
                Size = new System.Drawing.Size(16, 16),
                Text = "►",
                TextAlign = System.Drawing.ContentAlignment.MiddleCenter
            };

            group.Controls.Add(triggerBox);
            group.Controls.Add(arrow);
        }

        private void AddResetButton(GroupBox group, int id)
        {
            Button resetButton = new Button
            {
                BackColor = System.Drawing.Color.FromArgb(255, 224, 232),
                FlatStyle = FlatStyle.Flat,
                ForeColor = System.Drawing.Color.FromArgb(190, 32, 54),
                Location = new System.Drawing.Point(11, 73),
                Name = "btnResetMac" + id,
                Size = new System.Drawing.Size(54, 23),
                Text = "Reset",
                UseVisualStyleBackColor = false
            };
            resetButton.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(236, 160, 174);
            resetButton.Click += new EventHandler(this.onReset);

            group.Controls.Add(resetButton);
        }

        private void initializeLane(int id)
        {
            try
            {
                GroupBox p = (GroupBox)this.Controls.Find("chainGroup" + id, true)[0];
                foreach (Control control in p.Controls)
                {
                    ChainConfig chainConfig = ProfileSingleton.GetCurrent().MacroSwitch.chainConfigs.Find(config => config.id == id);

                    if (chainConfig == null) {
                        chainConfig = new ChainConfig(id, Key.None);
                        ProfileSingleton.GetCurrent().MacroSwitch.chainConfigs.Add(chainConfig);
                    }

                    if (control is TextBox)
                    {
                        TextBox textBox = (TextBox)control;
                        textBox.KeyDown += new System.Windows.Forms.KeyEventHandler(FormUtils.OnKeyDown);
                        textBox.KeyPress += new KeyPressEventHandler(FormUtils.OnKeyPress);
                        textBox.TextChanged += new EventHandler(this.onTextChange);
                    }

                    if (control is NumericUpDown)
                    {
                        NumericUpDown delayInput = (NumericUpDown)control;
                        delayInput.ValueChanged += new System.EventHandler(this.onDelayChange);
                    }


                    if (control is CheckBox)
                    {
                        CheckBox checkInput = (CheckBox)control;
                        checkInput.CheckedChanged += new System.EventHandler(this.onCheckClickChange);
                    }
                }
            }
            catch { }
        }
    }
}
