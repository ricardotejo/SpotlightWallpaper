using Microsoft.Win32;
using System;
using System.Linq;
using System.Security.Principal;

namespace SpotlightWallpaper
{
    internal class RegistryUtil
    {
        public (string contentId, string landscapeImage, string portraitImage) GetLastImages()
        {
            string SID = WindowsIdentity.GetCurrent().User.ToString();
            string keyPath = "SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Authentication\\LogonUI\\Creative\\" + SID;

            RegistryKey registryKey = RegistryKey.OpenBaseKey(RegistryHive.LocalMachine, RegistryView.Registry64);
            registryKey = registryKey.OpenSubKey(keyPath, writable: false);
            if (registryKey == null)
            {
                throw new Exception("Wrong Windows version or user not exists");
            }

            string[] subKeyNames = registryKey.GetSubKeyNames();
            string subkey = subKeyNames.LastOrDefault();
            if (subkey == null)
            {
                throw new Exception("No images");
            }

            RegistryKey subRegistryKey = registryKey.OpenSubKey(subkey, writable: false);

            subRegistryKey.GetValue("showImageOnSecureLock").ToString();
            return (
                subRegistryKey.GetValue("contentId").ToString(),
                subRegistryKey.GetValue("landscapeImage").ToString(),
                subRegistryKey.GetValue("portraitImage").ToString()
            );

        }

        private const string StartupTitle = "Wallpaper Changer";
        private const string RegistryRunKey = "SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Run";
        private const string RegistryThemeKey = @"Software\Microsoft\Windows\CurrentVersion\Themes\Personalize";

        public void InstallStartup()
        {
            RegistryKey rkApp = Registry.CurrentUser.OpenSubKey(RegistryRunKey, true);

            string appExec = "\"" + System.Reflection.Assembly.GetExecutingAssembly().Location + "\"";
            rkApp.SetValue(StartupTitle, appExec);

        }

        public void RemoveStartup()
        {
            RegistryKey rkApp = Registry.CurrentUser.OpenSubKey(RegistryRunKey, true);
            rkApp.DeleteValue(StartupTitle, false);
        }


        public WindowsTheme GetWindowsTheme()
        {
            RegistryKey rkApp = Registry.CurrentUser.OpenSubKey(RegistryThemeKey, false);

            object value = rkApp?.GetValue("AppsUseLightTheme");

            if (value == null) return WindowsTheme.Light;

            return ((int)value == 1) ? WindowsTheme.Light : WindowsTheme.Dark;
        }

        public enum WindowsTheme
        {
            Light,
            Dark
        }

    }
}
