using System;
using System.Drawing;
using System.Windows.Forms;
using _4RTools.Utils;
using _4RTools.Model;
using System.Media;
using _4RTools.Properties;
using System.IO;

namespace _4RTools.Forms
{
    public partial class ToggleApplicationStateForm : Form, IObserver
    {
        private Subject subject;
        private ContextMenu contextMenu;
        private MenuItem menuItem;
        private Image toggleOnImage;
        private Image toggleOffImage;
        private bool isRunning;

        //Store key used for last profile - necessarly to clean when change profile
        private Keys lastKey;

        public ToggleApplicationStateForm(Subject subject)
        {
            InitializeComponent();

            subject.Attach(this);
            this.subject = subject;
            KeyboardHook.Enable();
            this.txtStatusToggleKey.Text = ProfileSingleton.GetCurrent().UserPreferences.toggleStateKey;
            this.txtStatusToggleKey.KeyDown += new KeyEventHandler(FormUtils.OnKeyDown);
            this.txtStatusToggleKey.KeyPress += new KeyPressEventHandler(FormUtils.OnKeyPress);
            this.txtStatusToggleKey.TextChanged += new EventHandler(this.onStatusToggleKeyChange);
            this.Disposed += ToggleApplicationStateForm_Disposed;

            LoadToggleImages();
            SetToggleVisual(false);
            SetTrayIcon();
            InitializeContextualMenu();
        }

        private void ToggleApplicationStateForm_Disposed(object sender, EventArgs e)
        {
            toggleOnImage?.Dispose();
            toggleOffImage?.Dispose();
        }

        private void LoadToggleImages()
        {
            string onPath = ResolveAssetPath("assets", "images", "toggle_on.png");
            string offPath = ResolveAssetPath("assets", "images", "toggle_off.png");

            if (File.Exists(onPath))
            {
                toggleOnImage = Image.FromFile(onPath);
            }

            if (File.Exists(offPath))
            {
                toggleOffImage = Image.FromFile(offPath);
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

        private void SetToggleVisual(bool active)
        {
            isRunning = active;
            btnStatusToggle.Text = string.Empty;
            btnStatusToggle.BackColor = Color.Transparent;
            btnStatusToggle.Image = null;
            btnStatusToggle.BackgroundImage = active ? toggleOnImage : toggleOffImage;
            btnStatusToggle.BackgroundImageLayout = ImageLayout.Zoom;
            btnStatusToggle.FlatAppearance.MouseDownBackColor = Color.Transparent;
            btnStatusToggle.FlatAppearance.MouseOverBackColor = Color.Transparent;
        }

        private void SetTrayIcon()
        {
            this.notifyIconTray.Icon = Resources._4RTools.ETCResource.logo_4rtools_on;
        }

        private void InitializeContextualMenu()
        {
            this.contextMenu = new ContextMenu();
            this.menuItem = new MenuItem();

            this.contextMenu.MenuItems.AddRange(
                    new MenuItem[] { this.menuItem });

            this.menuItem.Index = 0;
            this.menuItem.Text = "Close";
            this.menuItem.Click += new EventHandler(this.notifyShutdownApplication);

            this.notifyIconTray.ContextMenu = this.contextMenu;
        }

        public void Update(ISubject subject)
        {
            MessageCode code = (subject as Subject).Message.code;
            if (code == MessageCode.PROFILE_CHANGED)
            {
                Keys currentToggleKey = (Keys)Enum.Parse(typeof(Keys), ProfileSingleton.GetCurrent().UserPreferences.toggleStateKey);
                KeyboardHook.Remove(lastKey); //Remove last key hook to prevent toggle with last profile key used.

                this.txtStatusToggleKey.Text = currentToggleKey.ToString();
                KeyboardHook.Add(currentToggleKey, new KeyboardHook.KeyPressed(this.toggleStatus));
                lastKey = currentToggleKey;
            }

            if (code == MessageCode.PROCESS_CHANGED || code == MessageCode.PROFILE_CHANGED || code == MessageCode.SESSION_STATE_CHANGED)
            {
                SyncSelectedSessionVisual();
            }
        }

        private void btnToggleStatusHandler(object sender, EventArgs e) { this.toggleStatus(); }

        private void onStatusToggleKeyChange(object sender, EventArgs e)
        {
            //Get last key from profile before update it in json
            Keys currentToggleKey = (Keys)Enum.Parse(typeof(Keys), this.txtStatusToggleKey.Text);
            KeyboardHook.Remove(lastKey);
            KeyboardHook.Add(currentToggleKey, new KeyboardHook.KeyPressed(this.toggleStatus));
            ProfileSingleton.GetCurrent().UserPreferences.toggleStateKey = currentToggleKey.ToString(); //Update profile key
            ProfileSingleton.SetConfiguration(ProfileSingleton.GetCurrent().UserPreferences);

            lastKey = currentToggleKey; //Refresh lastKey to update 
        }

        private bool toggleStatus()
        {
            ClientSession session = ClientSessionManager.Selected;
            if (isRunning)
            {
                session?.Stop();
                SetToggleVisual(false);
                SetTrayIcon();
                this.lblStatusToggle.Text = "Press the key to start!";
                this.subject.Notify(new Utils.Message(MessageCode.SESSION_STATE_CHANGED, session));

                if (this.cbAudio.Checked) { new SoundPlayer(Resources._4RTools.ETCResource.Speech_Off).Play(); }
            }
            else
            {
                if (session != null)
                {
                    session.Start();
                    SetToggleVisual(true);
                    SetTrayIcon();
                    this.lblStatusToggle.Text = "Press the key to stop!";
                    this.lblStatusToggle.ForeColor = Color.Black;
                    this.subject.Notify(new Utils.Message(MessageCode.SESSION_STATE_CHANGED, session));

                    if (this.cbAudio.Checked) { new SoundPlayer(Resources._4RTools.ETCResource.Speech_On).Play(); }
                }
                else
                {
                    this.lblStatusToggle.Text = "Select the Ragnarok Client";
                    this.lblStatusToggle.ForeColor = Color.Red;
                }
            }

            return true;
        }

        private void SyncSelectedSessionVisual()
        {
            ClientSession session = ClientSessionManager.Selected;
            bool active = session != null && session.IsRunning;
            SetToggleVisual(active);
            lblStatusToggle.Text = session == null
                ? "Select the Ragnarok Client"
                : active ? "Press the key to stop!" : "Press the key to start!";
            lblStatusToggle.ForeColor = session == null ? Color.Red : Color.Black;
        }

        private void notifyIconDoubleClick(object sender, MouseEventArgs e)
        {
            this.subject.Notify(new Utils.Message(MessageCode.CLICK_ICON_TRAY, null));
        }

        private void notifyShutdownApplication(object Sender, EventArgs e)
        {
            // Close the form, which closes the application.
            this.subject.Notify(new Utils.Message(MessageCode.SHUTDOWN_APPLICATION, null));
        }
    }
}
