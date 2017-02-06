using BackgroundImageSorter.Model;
using BackgroundImageSorter.View;
using NDesk.Options;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BackgroundImageSorter.Controller
{
    public class ConfigurationController
    {
        public static Configuration EstablishConfiguration(string[] args)
        {
            Configuration config = ConfigurationBuilder.BuildConfig();

            config = SetupConfiguration(args, config);
            return config;
        }


        public static Configuration SetupConfiguration(string[] args, Configuration config)
        {
            try
            {
                OptionSet options = SetOptions(config);

                ParseArguments(args, options);
                ConsiderHelp(config, options);
            }
            catch (OptionException e)
            {
                ConsoleView.DisplayErrorMessage(e);
                config.Error = true;
            }

            config = SetDefaultDirectories(config);
            ConfirmImportantFoldersExist(config);

            return config;
        }

        private static void ConfirmImportantFoldersExist(Configuration config)
        {
            if (!config.Destination.Exists || !config.Source.Exists)
            {
                ConsoleView.DisplayError();
                config.Error = true;
            }
        }

        private static void ConsiderHelp(Configuration config, OptionSet options)
        {
            if (config.ShowHelp)
            {
                ConsoleView.ShowHelp(options);
                config.Error = true;
            }
        }

        private static OptionSet SetOptions(Configuration config)
        {
            return new NDesk.Options.OptionSet() {
                { "d|DataFile=", "Dao Data File", d => config.DataFile = new FileInfo(d) },
                { "s|Source=", "Folder to Pull Images From", v => config.Source = new DirectoryInfo(v) },
                { "o|Output=",  "Folder to Place Sorted Images Into", v => config.Destination = new DirectoryInfo(v)},
                { "Single",  "Place All Scanned Files Directly Into Output Folder", v => config.Single = v != null},
                { "p|Portrait=",  "Folder to Place Portrait Images Into", v => config.Portrait = new DirectoryInfo(v)},
                { "l|Landscape=",  "Folder to Place Landscape Images Into", v => config.Landscape = new DirectoryInfo(v)},
                { "f|Fast", "Fast Scan - Use File Names Instead of Hashes.", v => config.FastScan = v != null },
                { "b|Background", "Only Copy Images Larger Than 1080x1080.", v => config.LargeImagesOnly = v != null },
                { "i|Image", "Only Copy Images - Unrecognized Files Ignored", v => config.ImagesOnly = v != null },
                { "t|Test", "Print Files Copied, But Do No Actual Copying.", v => config.Test = v != null },
                { "r|Rebuild", "Rebuild Image File Extensions.", v => config.RebuildExtensions = v != null },
                { "a|Automated", "Suppress the Report and Do Not Pause at the End\n\tUseful For Batch Scripts.", v => config.SuppressReport = v != null },
                { "A|Aggressive", "Aggressively ReBuild File Extensions.\n\tRemoves Everything After The First Dot(.).\n\tDefault is Last Dot.", v => config.AggressiveExtensions = v != null },
                { "sub|Sub", "Recurrsively Include Subdirectories In Search.", v => config.Recurse = v != null },
                { "pre|Prescan", "PreScan The Destination Directory.", v => config.PreScan = v != null },
                { "h|help",  "show this message and exit",
                        v => config.ShowHelp = v != null },
                { "?",  "show this message and exit",
                        v => config.ShowHelp = v != null }
            };
        }

        private static void ParseArguments(string[] args, OptionSet p)
        {
            List<string> extra = p.Parse(args);
            if (extra.Count > 0) throw new OptionException(extra.First<string>(), "Unidentified Option");
        }

        public static Configuration SetDefaultDirectories(Configuration config)
        {
            if (config.Source == null)
                config.Source = new DirectoryInfo(@"C:\Users\" + Environment.UserName + @"\AppData\Local\Packages\Microsoft.Windows.ContentDeliveryManager_cw5n1h2txyewy\LocalState\Assets");

            if (config.Destination == null)
                config.Destination = new DirectoryInfo(@"C:\Users\" + Environment.UserName + @"\Pictures\Saved Pictures");

            if (config.DataFile == null)
                config.DataFile = new FileInfo(@".\PhotoData.bin");

            if (config.Single)
            {
                config.BackgroundDirectory = config.Destination;
                config.DataDirectory = config.Destination;
                config.Portrait = config.Destination;
                config.SmallDirectory = config.Destination;
                config.Landscape = config.Destination;
            }

            if (config.BackgroundDirectory == null)
                config.BackgroundDirectory = PrepareSubDirectory(config.Destination, "Backgrounds");

            if (config.Landscape == null)
                config.Landscape = PrepareSubDirectory(config.BackgroundDirectory, "Landscape");

            if (config.Portrait == null)
                config.Portrait = PrepareSubDirectory(config.BackgroundDirectory, "Portrait");

            if (config.SmallDirectory == null)
                config.SmallDirectory = PrepareSubDirectory(config.Destination, "Other Images");

            if (config.DataDirectory == null)
                config.DataDirectory = PrepareSubDirectory(config.Destination, "Data");

            return config;
        }


        private static DirectoryInfo PrepareSubDirectory(DirectoryInfo directory, string subDirectoryTitle)
        {
            return new DirectoryInfo(directory.FullName + @"\" + subDirectoryTitle);
        }

    }
}
