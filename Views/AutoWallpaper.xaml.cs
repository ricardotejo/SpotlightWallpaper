using System;
using System.Windows;
using System.Windows.Media.Imaging;

namespace SpotlightWallpaper
{
    /// <summary>
    /// Interaction logic for AutoWallpaper.xaml
    /// </summary>
    public partial class AutoWallpaper
    {
        public AutoWallpaper()
        {
            InitializeComponent();
            thumbnailImage.Visibility = Visibility.Hidden;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            var desktopWorkingArea = System.Windows.SystemParameters.WorkArea;
            this.Left = desktopWorkingArea.Right - this.Width - 16;
            this.Top = desktopWorkingArea.Bottom - this.Height - 16;
        }

        private void Window_Deactivated(object sender, EventArgs e)
        {
            try
            {
                Close(); // Close when click on other application
            }
            catch
            {
                // INFO: Ignore error on Close() while window is closing 
            }
        }

        string contentId;
        string landscapeImage;
        string portraitImage;

        public void ShowAsk(string contentId, string landscapeImage, string portraitImage)
        {
            this.contentId = contentId;
            this.landscapeImage = landscapeImage;
            this.portraitImage = portraitImage;


            thumbnailImage.Source = new BitmapImage(new Uri(landscapeImage));
            thumbnailImage.Visibility = Visibility.Visible;
            this.Show();
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            new FileManager().CreateAlbum(contentId, landscapeImage, portraitImage);
            
            this.Close();
        }

        private void ApplyButton_Click(object sender, RoutedEventArgs e)
        {
            var fm = new FileManager();
            fm.CreateAlbum(contentId, landscapeImage, portraitImage);
            
            (string aL, string aP) = fm.GetAlbumImages(contentId);
            
            WallpaperManager.SetWallpapers(aL, aP);
            
            this.Close();
        }
    }
}
