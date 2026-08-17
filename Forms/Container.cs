using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using System.Diagnostics;
using System.IO;
using _4RTools.Model;
using _4RTools.Utils;

namespace _4RTools.Forms
{
    public partial class Container : Form, IObserver
    {

        private Subject subject = new Subject();
        private string currentProfile;
        private TextBox itemTransferKey;
        private ToolTip itemTransferToolTip;
        private Label clientSessionStatus;
        private static readonly Color AppSurface = Color.FromArgb(248, 251, 253);
        private static readonly Color AppPanel = Color.FromArgb(232, 244, 252);
        private static readonly Color AppPanelAlt = Color.FromArgb(218, 235, 247);
        private static readonly Color AppCard = Color.White;
        private static readonly Color AppLine = Color.FromArgb(179, 205, 222);
        private static readonly Color AppLineSoft = Color.FromArgb(214, 230, 240);
        private static readonly Color AppAccent = Color.FromArgb(39, 121, 169);
        private static readonly Color AppAccentDark = Color.FromArgb(30, 80, 118);
        private static readonly Color AppText = Color.FromArgb(34, 48, 58);
        private static readonly Color AppMutedText = Color.FromArgb(86, 108, 123);
        public Container()
        {
            this.subject.Attach(this);

            InitializeComponent();
            SetStyle(ControlStyles.AllPaintingInWmPaint | ControlStyles.OptimizedDoubleBuffer | ControlStyles.ResizeRedraw, true);
            ApplyUiPolish();
            AddItemTransferHelper();
            SetAppLogo();
            SetTopTabImages();
            this.Text = AppConfig.Name + " - " + AppConfig.Version; // Window title

            //Container Configuration
            this.IsMdiContainer = true;
            SetBackGroundColorOfMDIForm();

            //Paint Children Forms
            SetToggleApplicationStateWindow();
            SetAutopotWindow();
            SetSkillTimerWindow();
            SetProfileWindow();
            SetAHKWindow();
            SetAutobuffSkillWindow();
            SetAutobuffStuffWindow();
            SetDebuffRecoveryWindow();
            SetSongMacroWindow();
            SetATKDEFWindow();
            SetMacroSwitchWindow();
            SetPixelMacroWindow();
            SetServerWindow();

            TrackerSingleton.Instance().SendEvent("desktop_login", "page_view", "desktop_container_load");
        }

        public void addform(TabPage tp, Form f)
        {

            if (!tp.Controls.Contains(f))
            {
                ApplyChildUiPolish(f);
                tp.Controls.Add(f);
                f.Dock = DockStyle.Fill;
                f.Show();
                Refresh();
            }
            Refresh();
        }

        private void SetTopTabImages()
        {
            string autopotImagePath = ResolveAssetPath("assets", "images", "tab_autopot_hp.png");
            string skillTimerImagePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "assets", "images", "tab_skill_timer.png");

            if (!File.Exists(skillTimerImagePath))
            {
                skillTimerImagePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "..", "..", "assets", "images", "tab_skill_timer.png");
            }

            if (File.Exists(autopotImagePath) || File.Exists(skillTimerImagePath))
            {
                this.TabControlImageList.Images.Clear();
                this.TabControlImageList.ImageSize = new Size(16, 16);
                this.TabControlImageList.ColorDepth = ColorDepth.Depth32Bit;

                AddTopTabImage("tab_autopot_hp", autopotImagePath);
                AddTopTabImage("tab_skill_timer", skillTimerImagePath);

                if (this.TabControlImageList.Images.ContainsKey("tab_autopot_hp"))
                {
                    this.tabPageAutopot.ImageIndex = -1;
                    this.tabPageAutopot.ImageKey = "tab_autopot_hp";
                }

                if (this.TabControlImageList.Images.ContainsKey("tab_skill_timer"))
                {
                    this.tabSkillTimer.ImageIndex = -1;
                    this.tabSkillTimer.ImageKey = "tab_skill_timer";
                }
            }
        }

        private void AddTopTabImage(string key, string imagePath)
        {
            if (!File.Exists(imagePath) || this.TabControlImageList.Images.ContainsKey(key))
            {
                return;
            }

            using (Image source = Image.FromFile(imagePath))
            {
                this.TabControlImageList.Images.Add(key, CreateTabIcon(source));
            }
        }

        private void SetAppLogo()
        {
            string logoImagePath = ResolveAssetPath("assets", "images", "cookiecutter_logo.png");
            if (File.Exists(logoImagePath))
            {
                this.panelDiscImage.BackgroundImage = Image.FromFile(logoImagePath);
                this.panelDiscImage.BackgroundImageLayout = ImageLayout.Zoom;
                this.panelDiscImage.Cursor = Cursors.Hand;
            }

            string logoIconPath = ResolveAssetPath("assets", "etc", "logo_4rtools_on.ico");
            if (File.Exists(logoIconPath))
            {
                try
                {
                    this.Icon = new Icon(logoIconPath);
                }
                catch { }
            }
        }

        private static string ResolveAssetPath(params string[] parts)
        {
            string[] basePathParts = new string[parts.Length + 1];
            basePathParts[0] = AppDomain.CurrentDomain.BaseDirectory;
            parts.CopyTo(basePathParts, 1);

            string path = Path.Combine(basePathParts);
            if (File.Exists(path))
            {
                return path;
            }

            string[] sourcePathParts = new string[parts.Length + 3];
            sourcePathParts[0] = AppDomain.CurrentDomain.BaseDirectory;
            sourcePathParts[1] = "..";
            sourcePathParts[2] = "..";
            parts.CopyTo(sourcePathParts, 3);

            return Path.Combine(sourcePathParts);
        }

        private static Bitmap CreateTabIcon(Image source)
        {
            Bitmap icon = new Bitmap(16, 16);
            using (Graphics graphics = Graphics.FromImage(icon))
            {
                graphics.Clear(Color.Transparent);
                graphics.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.NearestNeighbor;
                graphics.PixelOffsetMode = System.Drawing.Drawing2D.PixelOffsetMode.Half;

                int size = Math.Min(16, Math.Max(source.Width, source.Height));
                int x = (16 - size) / 2;
                int y = (16 - size) / 2;
                graphics.DrawImage(source, new Rectangle(x, y, size, size));
            }

            return icon;
        }

        private void ApplyUiPolish()
        {
            Font uiFont = new Font("Segoe UI", 9F);

            this.BackColor = AppSurface;
            this.Font = uiFont;
            this.panelFooter.BackColor = AppCard;
            this.panel4.BackColor = AppLineSoft;
            this.panel5.BackColor = AppLineSoft;
            this.lbPowered.ForeColor = AppMutedText;
            this.lblProcessName.ForeColor = AppText;
            this.labelProfile.ForeColor = AppText;
            this.lblCharacterName.ForeColor = AppText;
            this.characterName.ForeColor = Color.FromArgb(28, 113, 75);
            this.lblLinkDiscord.LinkColor = AppAccent;
            this.lblLinkDiscord.ActiveLinkColor = AppAccentDark;
            this.OnOffPanel.BackColor = AppCard;
            this.OnOffPanel.BorderStyle = BorderStyle.None;
            ApplyRoundedRegion(this.OnOffPanel, 8);
            this.OnOffPanel.Paint -= PaintRoundedCard;
            this.OnOffPanel.Paint += PaintRoundedCard;
            this.panelFooter.BringToFront();
            this.OnOffPanel.BringToFront();

            StyleComboBox(this.processCB);
            StyleComboBox(this.profileCB);

            this.btnRefresh.FlatStyle = FlatStyle.Flat;
            this.btnRefresh.FlatAppearance.BorderColor = AppLine;
            this.btnRefresh.FlatAppearance.MouseOverBackColor = AppPanelAlt;
            this.btnRefresh.BackColor = AppCard;
            this.btnRefresh.Cursor = Cursors.Hand;
            ApplyRoundedRegion(this.btnRefresh, 5);

            StyleTabControl(this.tabControlAutopot, new Point(14, 6));
            StyleTabControl(this.atkDefMode, new Point(7, 3));
            RoundTabControlFrame(this.tabControlAutopot);
            RoundTabControlFrame(this.atkDefMode);
            this.tabDebuffRecovery.Text = "Debuffs";
        }

        private void AddItemTransferHelper()
        {
            Label itemTransferLabel = new Label
            {
                AutoSize = true,
                Font = new Font("Segoe UI", 9F, FontStyle.Bold),
                ForeColor = AppText,
                Location = new Point(494, 210),
                Name = "lblItemTransfer",
                Text = "Item Transfer"
            };

            itemTransferKey = new TextBox
            {
                Font = new Font("Segoe UI", 8.5F),
                Location = new Point(588, 207),
                Name = "txtItemTransferKey",
                Size = new Size(55, 23),
                Text = "None",
                TextAlign = HorizontalAlignment.Center
            };
            itemTransferKey.KeyDown += FormUtils.OnKeyDown;
            itemTransferKey.KeyPress += FormUtils.OnKeyPress;

            itemTransferToolTip = new ToolTip
            {
                IsBalloon = true,
                ToolTipIcon = ToolTipIcon.Info,
                ToolTipTitle = "Item Transfer Helper"
            };
            const string tooltipText = "Simulates Alt+Right Click for quick item transfer between storage and inventory";
            itemTransferToolTip.SetToolTip(itemTransferLabel, tooltipText);
            itemTransferToolTip.SetToolTip(itemTransferKey, tooltipText);

            Controls.Add(itemTransferLabel);
            Controls.Add(itemTransferKey);
            StyleTextBox(itemTransferKey);
            itemTransferLabel.BringToFront();
            itemTransferKey.BringToFront();

            clientSessionStatus = new Label
            {
                AutoEllipsis = true,
                Font = new Font("Segoe UI", 8.5F),
                ForeColor = AppMutedText,
                Location = new Point(366, 248),
                Name = "lblClientSessions",
                Size = new Size(277, 18)
            };
            Controls.Add(clientSessionStatus);
            clientSessionStatus.BringToFront();
            UpdateClientSessionStatus();
        }

        private void UpdateClientSessionStatus()
        {
            if (clientSessionStatus == null) { return; }

            int active = 0;
            foreach (ClientSession session in ClientSessionManager.Sessions)
            {
                if (session.IsRunning) { active++; }
            }

            ClientSession selected = ClientSessionManager.Selected;
            string selectedState = selected == null ? "No client selected" : selected.IsRunning ? "Selected ON" : "Selected OFF";
            clientSessionStatus.Text = "Active Clients: " + active + " | " + selectedState;
        }

        private static void StyleComboBox(ComboBox comboBox)
        {
            comboBox.BackColor = Color.White;
            comboBox.FlatStyle = FlatStyle.Flat;
            comboBox.Font = new Font("Segoe UI", 9F);
            comboBox.ForeColor = AppText;
            RoundControl(comboBox, 5);
        }

        private static void StyleTabControl(TabControl tabControl, Point tabPadding)
        {
            tabControl.Font = new Font("Segoe UI", 9F);
            tabControl.Padding = tabPadding;
            tabControl.Multiline = false;
            tabControl.SizeMode = TabSizeMode.Normal;
            tabControl.DrawMode = TabDrawMode.OwnerDrawFixed;
            tabControl.DrawItem -= DrawThemeTab;
            tabControl.DrawItem += DrawThemeTab;

            foreach (TabPage tabPage in tabControl.TabPages)
            {
                tabPage.BackColor = AppCard;
                tabPage.UseVisualStyleBackColor = false;
                tabPage.Padding = Padding.Empty;
            }
        }

        private static void RoundTabControlFrame(TabControl tabControl)
        {
            ApplyRoundedRegion(tabControl, 8);
            tabControl.SizeChanged += (sender, args) => ApplyRoundedRegion((Control)sender, 8);
        }

        private static void DrawThemeTab(object sender, DrawItemEventArgs e)
        {
            TabControl tabControl = (TabControl)sender;
            TabPage tabPage = tabControl.TabPages[e.Index];
            Rectangle bounds = tabControl.GetTabRect(e.Index);
            bool selected = e.Index == tabControl.SelectedIndex;
            Font tabFont = selected ? new Font(tabControl.Font, FontStyle.Bold) : tabControl.Font;

            Color fill = selected ? AppCard : AppPanelAlt;
            Color border = selected ? AppAccent : AppLineSoft;
            Color text = selected ? AppAccentDark : AppMutedText;

            bounds.Inflate(-1, -1);
            using (SolidBrush brush = new SolidBrush(fill))
            using (Pen pen = new Pen(border))
            using (GraphicsPath path = CreateRoundedRectanglePath(bounds, 6))
            {
                e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
                e.Graphics.FillPath(brush, path);
                e.Graphics.DrawPath(pen, path);
            }

            Image tabImage = null;
            if (!string.IsNullOrEmpty(tabPage.ImageKey) && tabControl.ImageList != null && tabControl.ImageList.Images.ContainsKey(tabPage.ImageKey))
            {
                tabImage = tabControl.ImageList.Images[tabPage.ImageKey];
            }

            if (tabImage == null)
            {
                TextRenderer.DrawText(e.Graphics, tabPage.Text, tabFont, bounds, text,
                    TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter | TextFormatFlags.EndEllipsis);
                if (selected) { tabFont.Dispose(); }
                return;
            }

            Size textSize = TextRenderer.MeasureText(tabPage.Text, tabFont);
            int gap = 5;
            int contentWidth = tabImage.Width + gap + textSize.Width;
            int imageX = bounds.Left + Math.Max(4, (bounds.Width - contentWidth) / 2);
            int imageY = bounds.Top + (bounds.Height - tabImage.Height) / 2;
            Rectangle textBounds = new Rectangle(imageX + tabImage.Width + gap, bounds.Top, bounds.Right - imageX - tabImage.Width - gap - 4, bounds.Height);

            e.Graphics.DrawImage(tabImage, imageX, imageY, tabImage.Width, tabImage.Height);
            TextRenderer.DrawText(
                e.Graphics,
                tabPage.Text,
                tabFont,
                textBounds,
                text,
                TextFormatFlags.Left | TextFormatFlags.VerticalCenter | TextFormatFlags.EndEllipsis);

            if (selected) { tabFont.Dispose(); }
        }

        private static void ApplyChildUiPolish(Control root)
        {
            root.BackColor = AppPanel;
            root.Font = new Font("Segoe UI", 9F);

            foreach (Control control in root.Controls)
            {
                if (control is GroupBox groupBox)
                {
                    groupBox.BackColor = AppCard;
                    groupBox.ForeColor = AppAccentDark;
                    groupBox.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
                    groupBox.FlatStyle = FlatStyle.Flat;
                    RoundControl(groupBox, 8);
                }
                else if (control is TextBox textBox)
                {
                    StyleTextBox(textBox);
                }
                else if (control is NumericUpDown numericUpDown)
                {
                    numericUpDown.BackColor = AppCard;
                    numericUpDown.ForeColor = AppText;
                    numericUpDown.Font = new Font("Segoe UI", 9F);
                    numericUpDown.BorderStyle = BorderStyle.None;
                    RoundControl(numericUpDown, 5);
                }
                else if (control is Button button && button.Name != "btnStatusToggle")
                {
                    button.BackColor = AppCard;
                    button.ForeColor = AppText;
                    button.FlatStyle = FlatStyle.Flat;
                    button.FlatAppearance.BorderColor = AppLineSoft;
                    button.FlatAppearance.MouseOverBackColor = AppPanelAlt;
                    button.FlatAppearance.MouseDownBackColor = Color.FromArgb(204, 226, 241);
                    button.Cursor = Cursors.Hand;
                    ApplyRoundedRegion(button, 6);
                }
                else if (control is ComboBox comboBox)
                {
                    StyleComboBox(comboBox);
                }
                else if (control is CheckBox checkBox)
                {
                    checkBox.ForeColor = AppText;
                    checkBox.FlatStyle = FlatStyle.Standard;
                    checkBox.BackColor = control.Parent is GroupBox ? AppCard : control.Parent.BackColor;
                }
                else if (control is DataGridView grid)
                {
                    StyleDataGrid(grid);
                }
                else if (control is ListBox listBox)
                {
                    listBox.BackColor = AppCard;
                    listBox.ForeColor = AppText;
                    listBox.BorderStyle = BorderStyle.FixedSingle;
                    RoundControl(listBox, 7);
                }
                else if (control is Panel panel)
                {
                    StylePanel(panel);
                }
                else if (control is Label label)
                {
                    label.ForeColor = AppText;
                }

                if (control.HasChildren)
                {
                    ApplyChildUiPolish(control);
                }
            }
        }

        private void SetBackGroundColorOfMDIForm()
        {
            foreach (Control ctl in this.Controls)
            {
                if ((ctl) is MdiClient)
                {
                    ctl.BackColor = AppSurface;
                }

            }
        }

        private static void StyleDataGrid(DataGridView grid)
        {
            grid.BackgroundColor = AppCard;
            grid.BorderStyle = BorderStyle.None;
            grid.GridColor = AppLineSoft;
            grid.EnableHeadersVisualStyles = false;
            grid.ColumnHeadersDefaultCellStyle.BackColor = AppPanelAlt;
            grid.ColumnHeadersDefaultCellStyle.ForeColor = AppText;
            grid.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 8.5F, FontStyle.Bold);
            grid.DefaultCellStyle.BackColor = AppCard;
            grid.DefaultCellStyle.ForeColor = AppText;
            grid.DefaultCellStyle.SelectionBackColor = Color.FromArgb(205, 229, 245);
            grid.DefaultCellStyle.SelectionForeColor = AppText;
            grid.RowHeadersDefaultCellStyle.BackColor = AppPanelAlt;
            RoundControl(grid, 7);
        }

        private static void StyleTextBox(TextBox textBox)
        {
            textBox.BackColor = AppCard;
            textBox.ForeColor = AppText;
            textBox.BorderStyle = BorderStyle.None;
            textBox.Font = new Font("Segoe UI", 9F);
            RoundControl(textBox, 5);
        }

        private static void RoundControl(Control control, int radius)
        {
            ApplyRoundedRegion(control, radius);
            control.SizeChanged -= RefreshRoundedControl;
            control.SizeChanged += RefreshRoundedControl;
            control.Tag = radius;
        }

        private static void RefreshRoundedControl(object sender, EventArgs e)
        {
            Control control = (Control)sender;
            int radius = control.Tag is int ? (int)control.Tag : 6;
            ApplyRoundedRegion(control, radius);
        }

        private static void StylePanel(Panel panel)
        {
            if (panel.Width <= 2 || panel.Height <= 2)
            {
                panel.BackColor = AppLineSoft;
                return;
            }

            if (panel.BorderStyle == BorderStyle.FixedSingle || panel.Name.StartsWith("panelMacro") || panel.Name.StartsWith("panelAd"))
            {
                panel.BackColor = AppCard;
                panel.BorderStyle = BorderStyle.None;
                ApplyRoundedRegion(panel, 7);
                panel.Paint -= PaintRoundedCard;
                panel.Paint += PaintRoundedCard;
            }
        }

        private static void PaintRoundedCard(object sender, PaintEventArgs e)
        {
            Control control = (Control)sender;
            Rectangle bounds = new Rectangle(0, 0, control.Width - 1, control.Height - 1);

            e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
            using (GraphicsPath path = CreateRoundedRectanglePath(bounds, 8))
            using (Pen pen = new Pen(AppLineSoft))
            {
                e.Graphics.DrawPath(pen, path);
            }
        }

        private static void ApplyRoundedRegion(Control control, int radius)
        {
            if (control.Width <= 0 || control.Height <= 0)
            {
                return;
            }

            Rectangle bounds = new Rectangle(0, 0, control.Width, control.Height);
            using (GraphicsPath path = CreateRoundedRectanglePath(bounds, radius))
            {
                Region oldRegion = control.Region;
                control.Region = new Region(path);
                oldRegion?.Dispose();
            }
        }

        private static GraphicsPath CreateRoundedRectanglePath(Rectangle bounds, int radius)
        {
            int diameter = radius * 2;
            GraphicsPath path = new GraphicsPath();

            if (diameter <= 0)
            {
                path.AddRectangle(bounds);
                path.CloseFigure();
                return path;
            }

            Rectangle arc = new Rectangle(bounds.Location, new Size(diameter, diameter));
            path.AddArc(arc, 180, 90);

            arc.X = bounds.Right - diameter;
            path.AddArc(arc, 270, 90);

            arc.Y = bounds.Bottom - diameter;
            path.AddArc(arc, 0, 90);

            arc.X = bounds.Left;
            path.AddArc(arc, 90, 90);

            path.CloseFigure();
            return path;
        }

        private void processCB_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (this.processCB.SelectedItem == null) { return; }
            Client client = new Client(this.processCB.SelectedItem.ToString());
            ClientSession session = ClientSessionManager.Select(client);
            if (session != null)
            {
                currentProfile = session.ProfileName;
                this.profileCB.SelectedItem = session.ProfileName;
            }
            subject.Notify(new Utils.Message(Utils.MessageCode.PROCESS_CHANGED, null));
            subject.Notify(new Utils.Message(Utils.MessageCode.PROFILE_CHANGED, null));
        }

        private void Container_Load(object sender, EventArgs e)
        {
            ProfileSingleton.Create("Default");
            this.refreshProcessList();
            this.refreshProfileList();
            this.profileCB.SelectedItem = "Default";
        }

        public void refreshProfileList()
        {
            this.Invoke((MethodInvoker)delegate ()
            {
                this.profileCB.Items.Clear();
            });
            foreach (string p in Profile.ListAll())
            {
                this.profileCB.Items.Add(p);
            }
        }

        private void refreshProcessList()
        {
            this.Invoke((MethodInvoker)delegate ()
            {
                this.processCB.Items.Clear();
            });
            foreach (Process p in Process.GetProcesses())
            {
                if (p.MainWindowTitle != "" && ClientListSingleton.ExistsByProcessName(p.ProcessName))
                {
                    this.processCB.Items.Add(string.Format("{0}.exe - {1}", p.ProcessName, p.Id));
                }
            }
        }

        private void btnRefresh_Click(object sender, EventArgs e)
        {
            this.refreshProcessList();
        }

        protected override void OnClosed(EventArgs e)
        {
            ShutdownApplication();
            base.OnClosed(e);
        }

        private void ShutdownApplication()
        {
            KeyboardHook.Disable();
            ClientSessionManager.StopAll();
            Environment.Exit(0);
        }

        private void lblLinkGithub_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            Process.Start(AppConfig.GithubLink);
        }

        private void lblLinkDiscord_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            Process.Start(AppConfig.DiscordLink);
        }

        private void websiteLinkLabel_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            Process.Start(AppConfig.Website);
        }

        private void profileCB_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (this.profileCB.Text != currentProfile)
            {
                try
                {
                    Profile selectedProfile = ProfileSingleton.LoadProfile(this.profileCB.Text);
                    if (ClientSessionManager.Selected != null)
                    {
                        ClientSessionManager.SetSelectedProfile(selectedProfile);
                    }
                    else
                    {
                        ProfileSingleton.Use(selectedProfile);
                    }
                    subject.Notify(new Utils.Message(MessageCode.PROFILE_CHANGED, null));
                    currentProfile = this.profileCB.Text.ToString();
                }
                catch (Exception ex)
                {
                    Console.Error.WriteLine($"[ProfileSingleton.Load] Error Message: {ex.Message}");
                    MessageBox.Show($"Error while loading the new profile. \nPlease get in touch via Discord. \nPlease send this error message to the admin: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        public void Update(ISubject subject)
        {
            switch ((subject as Subject).Message.code)
            {
                case MessageCode.PROCESS_CHANGED:
                case MessageCode.PROFILE_CHANGED:
                    Client client = ClientSingleton.GetClient();
                    if (client != null)
                        this.characterName.Text = client.ReadCharacterName();
                    UpdateClientSessionStatus();
                    break;
                case MessageCode.SESSION_STATE_CHANGED:
                    UpdateClientSessionStatus();
                    break;
                case MessageCode.SERVER_LIST_CHANGED:
                    this.refreshProcessList();
                    break;
                case MessageCode.CLICK_ICON_TRAY:
                    this.Show();
                    this.WindowState = FormWindowState.Normal;
                    break;
                case MessageCode.SHUTDOWN_APPLICATION:
                    this.ShutdownApplication();
                    break;
            }
        }

        private void containerResize(object sender, EventArgs e)
        {
            if (this.WindowState == FormWindowState.Minimized) { this.Hide(); }
        }

        #region Frames

        public void SetToggleApplicationStateWindow()
        {
            ToggleApplicationStateForm frm = new ToggleApplicationStateForm(subject);
            frm.FormBorderStyle = FormBorderStyle.None;
            frm.MdiParent = this;
            this.OnOffPanel.Controls.Add(frm);
            frm.Show();
        }

        public void SetAutopotWindow()
        {
            CombinedAutopotForm frm = new CombinedAutopotForm(subject);
            frm.FormBorderStyle = FormBorderStyle.None;
            frm.MdiParent = this;
            frm.Show();
            addform(this.tabPageAutopot, frm);
        }

        public void SetSkillTimerWindow()
        {
            SkillTimerForm frm = new SkillTimerForm(subject);
            frm.FormBorderStyle = FormBorderStyle.None;
            frm.MdiParent = this;
            frm.Show();
            addform(this.tabSkillTimer, frm);
        }

        public void SetProfileWindow()
        {
            ProfileForm frm = new ProfileForm(this);
            frm.FormBorderStyle = FormBorderStyle.None;
            frm.Location = new Point(0, 65);
            frm.MdiParent = this;
            frm.Show();
            addform(this.tabPageProfiles, frm);
        }

        public void SetServerWindow()
        {
            ServersForm frm = new ServersForm(subject);
            frm.FormBorderStyle = FormBorderStyle.None;
            frm.Location = new Point(0, 65);
            frm.MdiParent = this;
            frm.Show();
            addform(this.tabPageServer, frm);
        }

        public void SetAHKWindow()
        {
            AHKForm frm = new AHKForm(subject);
            frm.FormBorderStyle = FormBorderStyle.None;
            frm.Location = new Point(0, 65);
            frm.MdiParent = this;
            frm.Show();
            addform(this.tabPageSpammer, frm);
        }

        public void SetAutobuffSkillWindow()
        {
            SkillAutoBuffForm frm = new SkillAutoBuffForm(subject);
            frm.FormBorderStyle = FormBorderStyle.None;
            frm.Location = new Point(0, 65);
            frm.MdiParent = this;
            addform(this.tabPageAutobuffSkill, frm);
            frm.Show();
        }

        public void SetAutobuffStuffWindow()
        {
            StuffAutoBuffForm frm = new StuffAutoBuffForm(subject);
            frm.FormBorderStyle = FormBorderStyle.None;
            frm.Location = new Point(0, 65);
            frm.MdiParent = this;
            frm.Show();
            addform(this.tabPageAutobuffStuff, frm);
        }

        public void SetDebuffRecoveryWindow()
        {
            DebuffRecoveryForm frm = new DebuffRecoveryForm(subject);
            frm.FormBorderStyle = FormBorderStyle.None;
            frm.Location = new Point(0, 65);
            frm.MdiParent = this;
            frm.Show();
            addform(this.tabDebuffRecovery, frm);
        }

        public void SetSongMacroWindow()
        {
            MacroSongForm frm = new MacroSongForm(subject);
            frm.FormBorderStyle = FormBorderStyle.None;
            frm.Location = new Point(0, 65);
            frm.MdiParent = this;
            addform(this.tabPageMacroSongs, frm);
            frm.Show();
        }

        public void SetATKDEFWindow()
        {
            ATKDEFForm frm = new ATKDEFForm(subject);
            frm.FormBorderStyle = FormBorderStyle.None;
            frm.Location = new Point(0, 65);
            frm.MdiParent = this;
            addform(this.atkDef, frm);
            frm.Show();
        }

        public void SetMacroSwitchWindow()
        {
            MacroSwitchForm frm = new MacroSwitchForm(subject);
            frm.FormBorderStyle = FormBorderStyle.None;
            frm.Location = new Point(0, 65);
            frm.MdiParent = this;
            addform(this.tabMacroSwitch, frm);
            frm.Show();
        }

        public void SetPixelMacroWindow()
        {
            PixelMacroForm frm = new PixelMacroForm(subject);
            frm.FormBorderStyle = FormBorderStyle.None;
            frm.Location = new Point(0, 65);
            frm.MdiParent = this;
            addform(this.tabPixelMacro, frm);
            frm.Show();
        }

        #endregion
    }
}
