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
    }
}
