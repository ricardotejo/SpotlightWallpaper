using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media.Imaging;
using System.Linq;
using System.Collections.ObjectModel;

namespace SpotlightWallpaper
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        //public static readonly DependencyProperty ColumnsProperty = DependencyProperty.Register("Columns", typeof(int), typeof(MainWindow), new PropertyMetadata(3));
        //public ObservableCollection<PictureItem> Pictures { get; private set; } = new ObservableCollection<PictureItem>();
        public IEnumerable<PictureItem> Pictures { get; private set; }

        public MainWindow()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            var fm = new FileManager();

            //this.Pictures = new ObservableCollection<PictureItem>(fm.AvailablePictures().Select(a => new PictureItem(a)));
            this.Pictures = fm.AvailablePictures().Select(a => new PictureItem(a));
            Gallery.ItemsSource = Pictures;
        }



        private void Window_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            //Columns = (int)this.ActualWidth / 300; // 154 is a Tile's width
        }

        private void PictureButton_Click(object sender, RoutedEventArgs e)
        {
            FrameworkElement fe = sender as FrameworkElement;
            var contentId = ((PictureItem)fe.DataContext).ContentId;

            var fm = new FileManager();
            // TODO: refactor (repeated code x3)
            // Set Wallpaper
            (string aL, string aP) = fm.GetAlbumImages(contentId);
            WallpaperManager.SetWallpapers(aL, aP);
        }
    }

    public class PictureItem
    {
        public string ContentId { get; private set; }
        public BitmapImage Image { get; private set; }

        public PictureItem(string contentId)
        {
            this.ContentId = contentId;
            var fm = new FileManager();

            var bmi = new BitmapImage();
            bmi.BeginInit();
            bmi.CacheOption = BitmapCacheOption.OnLoad; 
            bmi.UriSource = new Uri(fm.AlbumThumbFile(contentId), UriKind.Absolute);
            bmi.EndInit();

            this.Image = bmi;

        }
    }
}
