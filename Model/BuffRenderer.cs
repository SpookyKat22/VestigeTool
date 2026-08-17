using _4RTools.Utils;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Dynamic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Input;

namespace _4RTools.Model
{
    internal class BuffRenderer
    {

        private readonly int BUFFS_PER_ROW = 5;
        private readonly int AUTOBUFFS_PER_ROW = 2;
        private readonly int COMPACT_COLUMN_WIDTH = 100;
        private readonly int LABELED_COLUMN_WIDTH = 252;
        private readonly int DISTANCE_BETWEEN_CONTAINERS = 10;
        private readonly int DISTANCE_BETWEEN_ROWS = 30;

        private string _modelName;
        private List<BuffContainer> _containers;
        private ToolTip _toolTip;

        public BuffRenderer(string model, List<BuffContainer> containers, ToolTip toolTip)
        {
            this._modelName = model;
            this._containers = containers;
            this._toolTip = toolTip;
        }

        public void doRender()
        {
            for (int i = 0; i < _containers.Count; i++)
            {
                BuffContainer bk = _containers[i];
                Point lastLocation = new Point(bk.container.Location.X, 20);
                int colCount = 0;

                if (i > 0)
                {
                    //If not first container to be rendered, get last container height and append 70
                    bk.container.Location = new Point(_containers[i - 1].container.Location.X, _containers[i - 1].container.Location.Y + _containers[i - 1].container.Height + DISTANCE_BETWEEN_CONTAINERS);
                }

                bool showAutobuffNames = this._modelName == "Autobuff";
                bool showDebuffNames = this._modelName == "DebuffsRecovery";
                int buffsPerRow = showAutobuffNames ? AUTOBUFFS_PER_ROW : showDebuffNames ? 2 : BUFFS_PER_ROW;
                int columnWidth = showAutobuffNames ? LABELED_COLUMN_WIDTH : showDebuffNames ? 310 : COMPACT_COLUMN_WIDTH;

                foreach (Buff skill in bk.skills)
                {
                    PictureBox pb = new PictureBox();
                    TextBox textBox = new TextBox();

                    pb.Image = skill.icon;
                    pb.SizeMode = PictureBoxSizeMode.CenterImage;
                    pb.BackgroundImageLayout = ImageLayout.Center;
                    pb.Location = new Point(lastLocation.X + (colCount * columnWidth), lastLocation.Y);
                    pb.Name = "pbox" + ((int)skill.effectStatusID);
                    pb.Size = new Size(26, 26);
                    _toolTip.SetToolTip(pb, skill.name);

                    textBox.KeyDown += new System.Windows.Forms.KeyEventHandler(FormUtils.OnKeyDown);
                    textBox.KeyPress += new KeyPressEventHandler(FormUtils.OnKeyPress);
                    textBox.TextChanged += new EventHandler(onTextChange);
                    textBox.Size = new Size(55, 20);
                    textBox.Tag = ((int)skill.effectStatusID);
                    textBox.Name = "in" + ((int)skill.effectStatusID);
                    textBox.Location = showDebuffNames
                        ? new Point(pb.Location.X + 166, pb.Location.Y + 3)
                        : new Point(pb.Location.X + 35, pb.Location.Y + 3);
                    textBox.TextAlign = HorizontalAlignment.Center;

                    bk.container.Controls.Add(textBox);
                    bk.container.Controls.Add(pb);

                    if (showAutobuffNames)
                    {
                        Label skillName = new Label();
                        skillName.AutoSize = true;
                        skillName.Location = new Point(textBox.Location.X + textBox.Width + 6, textBox.Location.Y + 3);
                        skillName.Name = "lbl" + ((int)skill.effectStatusID);
                        skillName.Text = skill.name;
                        skillName.Font = new Font("Segoe UI", 8.5F);
                        bk.container.Controls.Add(skillName);
                    }
                    else if (showDebuffNames)
                    {
                        Label skillName = new Label();
                        skillName.AutoSize = false;
                        skillName.AutoEllipsis = true;
                        skillName.Location = new Point(pb.Location.X + 34, pb.Location.Y + 5);
                        skillName.Name = "lbl" + ((int)skill.effectStatusID);
                        skillName.Size = new Size(125, 17);
                        skillName.Text = skill.name;
                        skillName.Font = new Font("Segoe UI", 8.5F);
                        bk.container.Controls.Add(skillName);
                    }

                    colCount++;

                    if (colCount == buffsPerRow)
                    {
                        colCount = 0;
                        lastLocation = new Point(bk.container.Location.X, lastLocation.Y + DISTANCE_BETWEEN_ROWS);
                    }
                }
            }
        }

        private void onTextChange(object sender, EventArgs e)
        {
            try
            {

                TextBox txtBox = (TextBox)sender;
                if (txtBox.Text.ToString() != String.Empty)
                {
                    Key key = (Key)Enum.Parse(typeof(Key), txtBox.Text.ToString());
                    EffectStatusIDs statusID = (EffectStatusIDs)Int16.Parse(txtBox.Name.Split(new[] { "in" }, StringSplitOptions.None)[1]);

                    switch (this._modelName)
                    {
                        case "Autobuff":
                            ProfileSingleton.GetCurrent().Autobuff.AddKeyToBuff(statusID, key);
                            ProfileSingleton.SetConfiguration(ProfileSingleton.GetCurrent().Autobuff);
                            break;
                        case "DebuffsRecovery":
                            ProfileSingleton.GetCurrent().DebuffsRecovery.AddKeyToBuff(statusID, key);
                            ProfileSingleton.SetConfiguration(ProfileSingleton.GetCurrent().DebuffsRecovery);
                            break;
                    }
                }
            }
            catch { }
        }

        public static void doUpdate(Dictionary<EffectStatusIDs, Key> autobuffDict, Control control)
        {
            FormUtils.ResetForm(control);
            foreach (EffectStatusIDs effect in autobuffDict.Keys)
            {
                Control[] c = control.Controls.Find("in" + (int)effect, true);
                if (c.Length > 0)
                {
                    TextBox textBox = (TextBox)c[0];
                    textBox.Text = autobuffDict[effect].ToString();
                }
            }
        }
    }
}
