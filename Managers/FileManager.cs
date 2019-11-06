using System;
using System.Linq;
using System.IO;
using System.Drawing;
using System.Collections.Generic;

namespace SpotlightWallpaper
{
    internal class FileManager
    {
        private const string DIR_PICTURES = "pictures";
        private string AlbumPath(string contentId)
        {

            return Path.Combine(PicturesDirectory, contentId);
        }

        private string PicturesDirectory
        {
            get
            {
                string basePath = System.AppDomain.CurrentDomain.BaseDirectory;
                return Path.Combine(basePath, DIR_PICTURES);
            }
        }

        public IEnumerable<string> AvailablePictures()
        {
            var di = new DirectoryInfo(PicturesDirectory);
            if (!di.Exists) { di.Create(); }

            return new DirectoryInfo(PicturesDirectory).GetDirectories().Select(a => a.Name);
        }

        public string AlbumThumbFile(string contentId)
        {
            return Path.Combine(AlbumPath(contentId), "thumb.jpg");
        }
        public bool ExistsAlbum(string contentId)
        {
            return Directory.Exists(AlbumPath(contentId));
        }
        public void CreateAlbum(string contentId, string landscapeImage, string portraitImage)
        {
            if (ExistsAlbum(contentId)) return;

            var dir = Directory.CreateDirectory(AlbumPath(contentId));
            File.Copy(landscapeImage, Path.Combine(dir.FullName, contentId + "_landscape.jpg"));
            File.Copy(portraitImage, Path.Combine(dir.FullName, contentId + "_portrait.jpg"));

            var thumb = Bitmap.FromFile(landscapeImage).GetThumbnailImage(260, 146, null, IntPtr.Zero); // () => { return false; }
            thumb.Save(AlbumThumbFile(contentId));
        }

        public (string landscapeImage, string portraitImage) GetAlbumImages(string contentId)
        {
            var album = AlbumPath(contentId);
            return (Path.Combine(album, contentId + "_landscape.jpg"),
                        Path.Combine(album, contentId + "_portrait.jpg"));
        }

    }
}
