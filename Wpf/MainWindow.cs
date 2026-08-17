using System;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Forms.Integration;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using _4RTools.Forms;
using _4RTools.Utils;

namespace _4RTools.Wpf
{
    public class MainWindow : Window
    {
        private readonly WindowsFormsHost legacyHost;
        private Container legacyContainer;

        public MainWindow()
        {
            Title = AppConfig.Name + " - " + AppConfig.Version;
            Width = 716;
            Height = 735;
            MinWidth = 716;
            MinHeight = 735;
            WindowStartupLocation = WindowStartupLocation.CenterScreen;
            Background = new SolidColorBrush(Color.FromRgb(248, 251, 253));
            SetWindowIcon();

            Border shellBorder = new Border
            {
                Background = new SolidColorBrush(Color.FromRgb(232, 244, 252)),
                BorderBrush = new SolidColorBrush(Color.FromRgb(179, 205, 222)),
                BorderThickness = new Thickness(1),
                CornerRadius = new CornerRadius(6),
                Padding = new Thickness(0),
                SnapsToDevicePixels = true
            };

            legacyHost = new WindowsFormsHost
            {
                Background = new SolidColorBrush(Color.FromRgb(248, 251, 253))
            };

            shellBorder.Child = legacyHost;
            Content = shellBorder;

            Loaded += MainWindow_Loaded;
            Closed += MainWindow_Closed;
        }

        private void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            legacyContainer = new Container();
            legacyContainer.IsMdiContainer = false;
            legacyContainer.TopLevel = false;
            legacyContainer.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            legacyContainer.Dock = System.Windows.Forms.DockStyle.Fill;

            legacyHost.Child = legacyContainer;
            legacyContainer.Show();
        }

        private void MainWindow_Closed(object sender, EventArgs e)
        {
            legacyHost.Child = null;
            legacyContainer?.Dispose();
            System.Windows.Forms.Application.Exit();
        }

        private void SetWindowIcon()
        {
            string iconPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "assets", "etc", "logo_4rtools_on.ico");
            if (!File.Exists(iconPath))
            {
                iconPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "..", "..", "assets", "etc", "logo_4rtools_on.ico");
            }

            if (File.Exists(iconPath))
            {
                Icon = BitmapFrame.Create(new Uri(Path.GetFullPath(iconPath)));
            }
        }
    }
}
