using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BackgroundImageSorter.Model
{
    class Report
    {
        public int Scanned { get; set; }
        public int Moved { get; set; }
        public int Skipped { get; set; }
        public int ImagesInLandscapeFolder { get; set; }
        public int StoredImages { get; set; }
        public int StoredFiles { get; set; }
        public int StoredBackgrounds { get; set; }

        public override string ToString()
        {
            return $"Files Scanned:\t{Scanned}\n" +
                $"Files Skipped:\t{Skipped}\n" + 
                $"Files Moved:\t{Moved}\n" + 
                $"Images in Background Folder:\t{ImagesInLandscapeFolder}\n" + 
                $"Stored Files:\t{StoredFiles}\n" +
                $"Store Images:\t{StoredImages}\n" + 
                $"Stored Backgrounds:\t{StoredBackgrounds}\n";
        }
    }
}
