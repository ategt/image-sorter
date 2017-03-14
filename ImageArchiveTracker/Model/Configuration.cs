using System.IO;

namespace ImageArchiveTracker.Model
{
    public class Configuration
    {
        public bool Add { get; set; }
        public bool Remove { get; set; }
        public bool List { get; set; }

        public bool Flickr { get; set; }
        public bool GooglePhoto { get; set; }
        public bool Disc { get; set; }

        public bool ForceDirectory { get; set; }
        public bool RecursiveSubdirectory { get; set; }

        public DirectoryInfo Move { get; set; }
        public DirectoryInfo Copy { get; set; }
    }
}
