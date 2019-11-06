using Microsoft.Win32;
using System;
using System.Windows;
using SpotlightWallpaper.Properties;
using MenuItem = System.Windows.Forms.MenuItem;

namespace SpotlightWallpaper
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        System.Windows.Forms.NotifyIcon nIcon;

        public App()
        {
            App.Current.ShutdownMode = ShutdownMode.OnExplicitShutdown;
        }

        protected override void OnStartup(StartupEventArgs e)
        {

            if (System.Diagnostics.Process.GetProcessesByName(
                System.IO.Path.GetFileNameWithoutExtension(System.Reflection.Assembly.GetEntryAssembly().Location)).Length > 1)
            {
                MessageBox.Show("Another instance of the application is running", "Spotlight Wallpaper", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                App.Current.Shutdown();
            }


            SetupNotifyIcon();
            SetupUnlockHandler();

            Execute();
        }

        private void SetupUnlockHandler()
        {
            SystemEvents.SessionSwitch += SystemEvents_SessionSwitch;
            SystemEvents.DisplaySettingsChanged += SystemEvents_DisplaySettingsChanged;
        }

        private void SystemEvents_SessionSwitch(object sender, SessionSwitchEventArgs e)
        {
            switch (e.Reason)
            {
                case SessionSwitchReason.SessionLogon:
                case SessionSwitchReason.SessionUnlock:
                    Execute();
                    break;
                case SessionSwitchReason.SessionLogoff:
                case SessionSwitchReason.SessionLock:
                    // Close all windows
                    for (int i = 0; i < Application.Current.Windows.Count; i++)
                    {
                        Application.Current.Windows[i].Close();
                    }
                    break;
            }
        }

        private void Execute()
        {
            var ru = new RegistryUtil();
            (string contentId, string landscapeImage, string portraitImage) = ru.GetLastImages();

            var fm = new FileManager();

            if (!fm.ExistsAlbum(contentId))
            {
                if (Settings.Default.AutoApply)
                {
                    fm.CreateAlbum(contentId, landscapeImage, portraitImage);

                    // TODO: refactor (repeated code)
                    // Set Wallpaper
                    (string aL, string aP) = fm.GetAlbumImages(contentId);
                    WallpaperManager.SetWallpapers(aL, aP);
                }
                else
                {
                    // Show UI for ask to set wallpaper
                    AskWallpaper(contentId, landscapeImage, portraitImage);
                }
            }
        }

        private void AskWallpaper(string contentId, string landscapeImage, string portraitImage)
        {
            AutoWallpaper aw = new AutoWallpaper();
            aw.ShowAsk(contentId, landscapeImage, portraitImage);
        }

        private void SystemEvents_DisplaySettingsChanged(object sender, EventArgs e)
        {
            throw new NotImplementedException();
        }

        private void SetupNotifyIcon()
        {
            //nIcon.ShowBalloonTip(5000, "Title", "Text", System.Windows.Forms.ToolTipIcon.Info);
            nIcon = new System.Windows.Forms.NotifyIcon();
            nIcon.Text = "Spotlight Wallpaper";
            nIcon.Icon = SpotlightWallpaper.Properties.Resources.TrayIcon;
            nIcon.DoubleClick += NIcon_Open;

            nIcon.ContextMenu = new System.Windows.Forms.ContextMenu();
            var mShow = new MenuItem("&Show", NIcon_Open) { DefaultItem = true };
            var mExit = new MenuItem("&Exit", NIcon_Exit);
            var mAuto = new MenuItem("&Auto apply", NIcon_SwapApply) { Checked = Settings.Default.AutoApply };
            var mSep= new MenuItem("-");
            var mRun = new MenuItem("&Run on start", NIcon_RunStart) { Checked = Settings.Default.RunOnStart };
            nIcon.ContextMenu.MenuItems.AddRange(new MenuItem[] { mShow, mAuto, mRun, mSep, mExit, });

            nIcon.Visible = true;
        }

        private void NIcon_RunStart(object sender, EventArgs e)
        {
            bool newStart = !Settings.Default.RunOnStart;
            (sender as MenuItem).Checked = newStart;
            Settings.Default.RunOnStart = newStart;
            Settings.Default.Save();

            if (newStart)
            {
                new RegistryUtil().InstallStartup();
            }
            else
            {
                new RegistryUtil().RemoveStartup();
            }
        }

        private void NIcon_SwapApply(object sender, EventArgs e)
        {
            bool newApply = !Settings.Default.AutoApply;
            (sender as MenuItem).Checked = newApply;
            Settings.Default.AutoApply = newApply;
            Settings.Default.Save();

        }

        private void NIcon_Open(object sender, EventArgs e)
        {
            new MainWindow().Show();
        }

        private void NIcon_Exit(object sender, EventArgs e)
        {
            nIcon.Visible = false;
            nIcon.Dispose();
            App.Current.Shutdown();
        }



    }
}
