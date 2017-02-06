using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BackgroundImageSorter.Model
{
    public class Configuration
    {
        public DirectoryInfo Source { get; set; }
        public DirectoryInfo Destination { get; set; }
        public DirectoryInfo Portrait { get; set; }
        public DirectoryInfo Landscape { get; set; }
        public DirectoryInfo BackgroundDirectory { get; set; }
        public DirectoryInfo SmallDirectory { get; set; }
        public DirectoryInfo DataDirectory { get; set; }
        public FileInfo DataFile { get; set; }
        public bool FastScan { get; set; }
        public bool ImagesOnly { get; set; }
        public bool LargeImagesOnly { get; set; }
        public bool Test { get; set; }
        public bool ShowHelp { get; set; }
        public bool Error { get; set; }
        public bool SuppressReport { get; set; }
        public bool NoUpdate { get; set; }
        public bool RebuildExtensions { get; set; }
        public bool AggressiveExtensions { get; set; }
        public bool Recurse { get; set; }
        public bool PreScan { get; set; }
        public bool Single { get; set; }
        public bool Move { get; set; }
    }
}
