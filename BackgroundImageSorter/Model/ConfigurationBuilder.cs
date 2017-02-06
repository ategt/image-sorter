using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BackgroundImageSorter.Model
{
    public class ConfigurationBuilder
    {
        public static Configuration BuildConfig()
        {
            return new Configuration
            {
                FastScan = false,
                LargeImagesOnly = false,
                ImagesOnly = false,
                ShowHelp = false,
                Test = false,
                Error = false,
                SuppressReport = false,
                RebuildExtensions = false,
                Recurse = false,
                Single = false
            };
        }
    }
}
