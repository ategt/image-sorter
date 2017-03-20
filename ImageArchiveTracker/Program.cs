using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImageArchiveTracker
{
    public class Program
    {
        public static void Main(string[] args)
        {
            new ImageArchiveTracker.Controller.Application(new Model.ArchiveDao("MediaArchive.sqlite3")).ProcessFile(args);
        }
    }
}
