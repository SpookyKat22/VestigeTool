using System;
using System.Drawing;
using System.IO;
using _4RTools.Model;
using System.Windows.Forms;

namespace _4RTools.Forms
{
    public partial class ProfileForm: Form
    {
        private Container container;
        public ProfileForm(Container container)
        {
            InitializeComponent();
            this.container = container;
            SetProfileButtonIcons();
            this.RefreshProfileList();
        }

        private void SetProfileButtonIcons()
        {
            SetButtonIcon(this.btnRemoveProfile, "delete.png");
            SetButtonIcon(this.btnEditProfile, "edit.png");
            SetButtonIcon(this.btnCopy, "copy.png");
        }

        private static void SetButtonIcon(Button button, string iconName)
        {
            string path = ResolveAssetPath("assets", "images", iconName);
            if (!File.Exists(path)) { return; }

            using (Image source = Image.FromFile(path))
            {
                button.Image = CreateButtonIcon(source);
            }

            button.ImageAlign = ContentAlignment.MiddleLeft;
            button.TextAlign = ContentAlignment.MiddleCenter;
            button.TextImageRelation = TextImageRelation.ImageBeforeText;
            button.Padding = new Padding(3, 0, 2, 0);
        }

        private static Bitmap CreateButtonIcon(Image source)
        {
            const int size = 13;
            Bitmap icon = new Bitmap(size, size);

            using (Graphics graphics = Graphics.FromImage(icon))
            {
                graphics.Clear(Color.Transparent);
                graphics.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.NearestNeighbor;
                graphics.PixelOffsetMode = System.Drawing.Drawing2D.PixelOffsetMode.Half;
                graphics.DrawImage(source, new Rectangle(0, 0, size, size));
            }

            return icon;
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

        private void RefreshProfileList()
        {
            foreach (string profile in Profile.ListAll())
            {
                int profileIndex = this.lbProfilesList.Items.IndexOf(profile);
                if (profile != "Default" && profileIndex == -1) { this.lbProfilesList.Items.Add(profile); };
            }
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            string newProfileName = this.txtProfileName.Text;
            if (string.IsNullOrEmpty(newProfileName)) { return; }

            ProfileSingleton.Create(newProfileName);
            this.RefreshProfileList();
            this.container.refreshProfileList();
            this.txtProfileName.Text = ""; // clear text box
        }

        private void btnRemoveProfile_Click(object sender, EventArgs e)
        {
            if (this.lbProfilesList.SelectedItem == null)
            {
                MessageBox.Show("No profile found! To delete a profile, first select an option from the Profile list.");
                return;
            }

            string selectedProfile = this.lbProfilesList.SelectedItem.ToString();
            if (selectedProfile == "Default")
            {
                MessageBox.Show("Cannot delete the Default profile!");
            } else
            {
                ProfileSingleton.Delete(selectedProfile);
                this.lbProfilesList.Items.Remove(selectedProfile);
                this.RefreshProfileList();
                this.container.refreshProfileList();
            }
        }

        private void btnEdit_Click(object sender, EventArgs e)
        {
            if (this.lbProfilesList.SelectedItem == null)
            {
                MessageBox.Show("To edit a profile, first select one from the Profile list.");
                return;
            }

            var selectedProfile = this.lbProfilesList.Items[this.lbProfilesList.SelectedIndex].ToString();

            if (selectedProfile == "Default")
            {
                MessageBox.Show("Cannot delete the Default profile!");
            }
            else {
                EditProfileName editProfileName = new EditProfileName();
                editProfileName.SetProfileName(selectedProfile);
                editProfileName.ShowDialog();

                if (editProfileName.DialogResult == DialogResult.OK) {
                    this.RefreshProfileList();
                    this.container.refreshProfileList();
                };
            }
        }

        private void btnCopy_Click(object sender, EventArgs e)
        {
            if (this.lbProfilesList.SelectedItem == null)
            {
                MessageBox.Show("To copy a profile, first select one from the Profile list.");
                return;
            }

            var selectedProfile = this.lbProfilesList.Items[this.lbProfilesList.SelectedIndex].ToString();
            if (selectedProfile == "Default")
            {
                MessageBox.Show("Cannot delete the Default profile!");
            }
            else {
                ProfileSingleton.Copy(selectedProfile);
                this.RefreshProfileList();
                this.container.refreshProfileList();
            }
        }
    }
}
