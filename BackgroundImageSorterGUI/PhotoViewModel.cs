using BackgroundImageSorter;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BackgroundImageSorterGUI
{
    public class PhotoViewModel
    {
        public List<Photo> Photos { get; }

        public PhotoViewModel()
        {
            this.Photos = new List<Photo>();
        }

        public System.Windows.Media.ImageSource RandomImage()
        {
            //System.Windows.Media.ImageSource.
            //System.Drawing.Image.FromFile( Photos[new Random().Next(0, Photos.Count)].FileInfo.FullName));
            //System.Windows.Media.ImageSource.

            //new Uri("Photos[new Random().Next(0, Photos.Count)].FileInfo.FullName"
            string path = Photos[new Random().Next(0, Photos.Count)].FileInfo.FullName;
            Uri uri = new Uri(path);
            return new System.Windows.Media.Imaging.BitmapImage(uri);
        }

    }
}
