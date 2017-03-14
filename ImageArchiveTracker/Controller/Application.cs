using ImageArchiveTracker.Model;
using NDesk.Options;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace ImageArchiveTracker.Controller
{
    public class Application
    {
        public ArchiveDao archiveDao = null;

        public void ProcessFile(string[] args)
        {

            OptionSet options = SetOptions(new Configuration());

            List<string> extras = options.Parse(args);

            List<FileInfo> filesToProcess = new Lis
                extras.ForEach(line => new FileInfo(line));
        }

        private static OptionSet SetOptions(Configuration config)
        {
            return new NDesk.Options.OptionSet() {
                { "a|Add", "Add File Hash to Database With Attributes If Specified.", v => config.Add = v != null },
                { "l|List", "List Attributes of Hash.", v => config.List = v != null },
                { "r|Remove", "Remove Attributes From Associated File Hash.", v => config.Remove = v != null },

                { "f|Flickr", "Flickr Attribute.", v => config.Flickr = v != null },
                { "g|Google", "Google Photos Attribute.", v => config.GooglePhoto = v != null },
                { "b|Both", "Both Google Photos And Flickr But Not Disc.", v => { config.Flickr = v != null; config.GooglePhoto = v != null; }},
                { "d|Disc", "Media File Archived To Disc.", v => config.Disc = v != null },

                { "m|Move=", "Move Files Processed Or Listed To Specified Directory.", v => config.Move = new DirectoryInfo(v) },
                { "c|Copy=", "Copy Files Processed Or Listed To Specified Directory.", v => config.Copy = new DirectoryInfo(v) },

                { "h|help",  "show this message and exit",
                        v => { throw new ShowHelpException(); } },
                { "?",  "show this message and exit",
                        v => { throw new ShowHelpException(); } }
            };
        }
        
        public static byte[] GetSHA512Hash(string path)
        {
            byte[] hash;

            using (FileStream stream = File.OpenRead(path))
            {
                SHA512Managed sha = new SHA512Managed();
                hash = sha.ComputeHash(stream);
            }

            return hash;
        }

    }
}
