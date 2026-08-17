using System;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using _4RTools.Utils;
using _4RTools.Model;
using System.Windows.Input;
using System.Collections.Generic;

namespace _4RTools.Forms
{
    public partial class SkillAutoBuffForm : Form, IObserver
    {

        private List<BuffContainer> skillContainers = new List<BuffContainer>();
        private readonly Dictionary<GroupBox, int> expandedSkillGroupHeights = new Dictionary<GroupBox, int>();
        private Image collapseArrow;
        private Image expandArrow;

        public SkillAutoBuffForm(Subject subject)
        {
            this.KeyPreview = true;
            InitializeComponent();

            skillContainers.Add(new BuffContainer(this.ArcherSkillsGP, Buff.GetArcherSkills()));
            skillContainers.Add(new BuffContainer(this.SwordmanSkillGP, Buff.GetSwordmanSkill()));
            skillContainers.Add(new BuffContainer(this.MageSkillGP, Buff.GetMageSkills()));
            skillContainers.Add(new BuffContainer(this.MerchantSkillsGP, Buff.GetMerchantSkills()));
            skillContainers.Add(new BuffContainer(this.ThiefSkillsGP, Buff.GetThiefSkills()));
            skillContainers.Add(new BuffContainer(this.AcolyteSkillsGP, Buff.GetAcolyteSkills()));
            skillContainers.Add(new BuffContainer(this.PadawanSkillsGP, Buff.GetPadawanSkills()));
            skillContainers.Add(new BuffContainer(this.TKSkillGroupBox, Buff.GetTaekwonSkills()));
            skillContainers.Add(new BuffContainer(this.NinjaSkillsGP, Buff.GetNinjaSkills()));
            skillContainers.Add(new BuffContainer(this.GunsSkillsGP, Buff.GetGunsSkills()));
            new BuffRenderer("Autobuff", skillContainers, toolTip1).doRender();
            LoadCollapseImages();
            AddCollapseButtons();
            LayoutSkillGroups();
            subject.Attach(this);
        }

        private void LoadCollapseImages()
        {
            string upPath = ResolveAssetPath("assets", "images", "sort_arrow_up.png");
            string downPath = ResolveAssetPath("assets", "images", "sort_arrow_down.png");

            if (File.Exists(upPath)) { collapseArrow = Image.FromFile(upPath); }
            if (File.Exists(downPath)) { expandArrow = Image.FromFile(downPath); }
        }

        private void AddCollapseButtons()
        {
            foreach (BuffContainer skillContainer in skillContainers)
            {
                GroupBox group = skillContainer.container;
                expandedSkillGroupHeights[group] = group.Height;

                PictureBox toggle = new PictureBox
                {
                    Cursor = System.Windows.Forms.Cursors.Hand,
                    Image = collapseArrow,
                    Location = new Point(group.Width - 18, 5),
                    Name = group.Name + "Collapse",
                    Size = new Size(12, 12),
                    SizeMode = PictureBoxSizeMode.CenterImage,
                    Tag = true
                };
                toggle.Click += ToggleSkillGroup;
                group.Controls.Add(toggle);
                toggle.BringToFront();
                toolTip1.SetToolTip(toggle, "Collapse " + group.Text);
            }
        }

        private void ToggleSkillGroup(object sender, EventArgs e)
        {
            PictureBox toggle = (PictureBox)sender;
            GroupBox group = (GroupBox)toggle.Parent;
            bool expanded = (bool)toggle.Tag;

            foreach (Control child in group.Controls)
            {
                if (child != toggle)
                {
                    child.Visible = !expanded;
                }
            }

            group.AutoSize = false;
            group.Height = expanded ? 24 : expandedSkillGroupHeights[group];
            toggle.Tag = !expanded;
            toggle.Image = expanded ? expandArrow : collapseArrow;
            toolTip1.SetToolTip(toggle, (expanded ? "Expand " : "Collapse ") + group.Text);
            LayoutSkillGroups();
        }

        private void LayoutSkillGroups()
        {
            SuspendLayout();
            AutoScrollPosition = Point.Empty;

            int top = 12;
            foreach (BuffContainer skillContainer in skillContainers)
            {
                GroupBox group = skillContainer.container;
                group.Location = new Point(group.Location.X, top);
                top += group.Height + 10;
            }

            AutoScrollMinSize = new Size(0, top);
            ResumeLayout();
        }

        private static string ResolveAssetPath(params string[] parts)
        {
            string[] buildParts = new string[parts.Length + 1];
            buildParts[0] = AppDomain.CurrentDomain.BaseDirectory;
            parts.CopyTo(buildParts, 1);

            string path = Path.Combine(buildParts);
            if (File.Exists(path)) { return path; }

            string[] sourceParts = new string[parts.Length + 3];
            sourceParts[0] = AppDomain.CurrentDomain.BaseDirectory;
            sourceParts[1] = "..";
            sourceParts[2] = "..";
            parts.CopyTo(sourceParts, 3);
            return Path.Combine(sourceParts);
        }

        public void Update(ISubject subject)
        {
            switch ((subject as Subject).Message.code)
            {
                case MessageCode.PROFILE_CHANGED:
                    BuffRenderer.doUpdate(new Dictionary<EffectStatusIDs, Key>(ProfileSingleton.GetCurrent().Autobuff.buffMapping), this);
                    break;
            }
        }
    }
}
