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

        public Application(ArchiveDao archiveDao)
        {
            this.archiveDao = archiveDao;
        }

        public void ProcessFile(string[] args)
        {
            OptionSet options = null;

            try
            {
                Configuration config = new Configuration();
                options = SetOptions(config);
                List<string> extras = options.Parse(args);

                //if (extras.Count < 1)
                //{
                //    // list all files matching criteria
                //    // 
                //}

                List<FileInfo> filesToProcess = extras.ConvertAll<FileInfo>(line => new FileInfo(line));

                filesToProcess.ForEach(fileToProcess =>
                           {
                               Archive archive = new Archive
                               {
                                   Hash = Application.GetSHA512Hash(fileToProcess.FullName),
                                   Disc = config.Disc,
                                   Flickr = config.Flickr,
                                   GooglePhoto = config.GooglePhoto
                               };

                               if (config.List)
                               {
                                   Archive dbArchive = archiveDao.Get(archive.Hash);

                                   Console.WriteLine(fileToProcess.Name + "\t" + dbArchive);

                                   //if (config.Flickr)
                                   //{
                                   //    if (archiveDao.IsOnFlickr(archive.Hash))
                                   //    {
                                   //        Console.WriteLine(fileToProcess);
                                   //    }
                                   //}
                               }
                               else if (config.Test)
                               {
                                   Archive dbArchive = archiveDao.Get(archive.Hash);

                                   if (
                                       object.Equals(dbArchive.GooglePhoto, archive.GooglePhoto) &&
                                       object.Equals(dbArchive.Disc, archive.Disc) &&
                                       object.Equals(dbArchive.Flickr, archive.Flickr) &&
                                       object.Equals(dbArchive.Hash, archive.Hash))
                                   {
                                       Console.WriteLine("Pass");
                                   }
                                   else
                                   {
                                       Console.WriteLine(fileToProcess.FullName);                                    
                                   }
                               }
                           }
                    );
            }
            catch (ShowHelpException ex)
            {
                ShowHelp(options);
            }
        }

        public static void ShowHelp(OptionSet p)
        {
            Console.WriteLine("Usage: [OPTIONS]+ File1 [File2 [...]]");
            Console.WriteLine();
            Console.WriteLine("Options:");
            p.WriteOptionDescriptions(Console.Out);
            Console.WriteLine();
        }

        private static OptionSet SetOptions(Configuration config)
        {
            return new NDesk.Options.OptionSet() {
                { "a|Add", "Add File Hash to Database With Attributes If Specified.", v => config.Add = v != null },
                { "l|List", "List Attributes of Hash.", v => config.List = v != null },
                { "r|Remove", "Remove Attributes From Associated File Hash.", v => config.Remove = v != null },
                { "t|Test", "Displays Files That Do Not Agree With Database Information.", v => config.Test = v != null },

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
