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
        public int Images { get; set; }
        public int StoredImages { get; set; }
        public int StoredFiles { get; set; }
        public int StoredBackgrounds { get; set; }

        public override string ToString()
        {
            return $"Files Scanned:\t{Scanned}\nFiles Skipped:\t{Skipped}\nFiles Moved:\t{Moved}\nImages in Background Folder:\t{Images}\nStored Files:\t{StoredFiles}\nStore Images:\t{StoredImages}\nStored Backgrounds:\t{StoredBackgrounds}\n";
        }
    }
}
