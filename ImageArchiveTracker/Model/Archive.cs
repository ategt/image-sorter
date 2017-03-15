using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImageArchiveTracker.Model
{
    public class Archive
    {
        public byte[] Hash { get; set; }
        public bool Flickr { get; set; }
        public bool GooglePhoto { get; set; }
        public bool Disc { get; set; }

        public override string ToString()
        {
            return Hash.GetHashCode() + ", \t" + Flickr + ", \t" + GooglePhoto + ", \t" + Disc;
        }
    }
}
