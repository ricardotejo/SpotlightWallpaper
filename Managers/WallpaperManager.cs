using System.IO;
using System.Runtime.InteropServices;
using System;
using static WinAPI.DesktopWallpaper;

namespace WinAPI
{
    public class DesktopWallpaper
    {
        [StructLayout(LayoutKind.Sequential)]
        public struct Rect
        {
            public int Left;
            public int Top;
            public int Right;
            public int Bottom;
        }

        public enum DesktopSlideshowOptions
        {
            ShuffleImages = 0x01
        }

        public enum DesktopSlideshowState
        {
            Enabled = 0x01,
            Slideshow = 0x02,
            DisabledByRemoteSession = 0x04
        }

        public enum DesktopSlideshowDirection
        {
            Forward = 0,
            Backward = 1
        }

        public enum DesktopWallpaperPosition
        {
            Center = 0,
            Tile = 1,
            Stretch = 2,
            Fit = 3,
            Fill = 4,
            Span = 5
        }

        [ComImport, Guid("B92B56A9-8B55-4E14-9A89-0199BBB6F93B"), InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
        public interface IDesktopWallpaper
        {
            void SetWallpaper([MarshalAs(UnmanagedType.LPWStr)] string monitorID, [MarshalAs(UnmanagedType.LPWStr)] string wallpaper);

            [return: MarshalAs(UnmanagedType.LPWStr)]
            string GetWallpaper([MarshalAs(UnmanagedType.LPWStr)] string monitorID);

            [return: MarshalAs(UnmanagedType.LPWStr)]
            string GetMonitorDevicePathAt(uint monitorIndex);

            [return: MarshalAs(UnmanagedType.U4)]
            uint GetMonitorDevicePathCount();

            [return: MarshalAs(UnmanagedType.Struct)]
            Rect GetMonitorRECT([MarshalAs(UnmanagedType.LPWStr)] string monitorID);

            void SetBackgroundColor([MarshalAs(UnmanagedType.U4)] uint color);

            [return: MarshalAs(UnmanagedType.U4)]
            uint GetBackgroundColor();

            void SetPosition([MarshalAs(UnmanagedType.I4)] DesktopWallpaperPosition position);

            [return: MarshalAs(UnmanagedType.I4)]
            DesktopWallpaperPosition GetPosition();

            void SetSlideshow(IntPtr items);

            IntPtr GetSlideshow();

            void SetSlideshowOptions(DesktopSlideshowDirection options, uint slideshowTick);
            [PreserveSig]

            uint GetSlideshowOptions(out DesktopSlideshowDirection options, out uint slideshowTick);

            void AdvanceSlideshow([MarshalAs(UnmanagedType.LPWStr)] string monitorID, [MarshalAs(UnmanagedType.I4)] DesktopSlideshowDirection direction);

            DesktopSlideshowDirection GetStatus();

            bool Enable();
        }
    }
}
namespace SpotlightWallpaper
{
    public class WallpaperManager
    {
        static readonly Guid CLSID_DesktopWallpaper = new Guid("{C2CF3110-460E-4fc1-B9D0-8A1C0C9CC4BD}");

        public static IDesktopWallpaper GetWallpaper()
        {
            Type typeDesktopWallpaper = Type.GetTypeFromCLSID(CLSID_DesktopWallpaper);
            return (IDesktopWallpaper)Activator.CreateInstance(typeDesktopWallpaper);
        }

        public static void SetWallpapers(string landscapeImage, string portraitImage)
        {
            var wp = GetWallpaper();
            uint count = wp.GetMonitorDevicePathCount();
            for (uint i = 0; i < count; i++)
            {
                var mid = wp.GetMonitorDevicePathAt(i);
                Rect mrect = wp.GetMonitorRECT(mid);
                string image = (mrect.Right - mrect.Left) / (float)(mrect.Bottom - mrect.Top) > 1.0f ? landscapeImage : portraitImage;

                if (File.Exists(image))
                {
                    Console.WriteLine(image);
                    wp.SetWallpaper(mid, image);
                    wp.SetPosition(DesktopWallpaperPosition.Fill);
                }
            }
        }
    }

}
