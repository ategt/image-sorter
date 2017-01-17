using BackgroundImageSorter;
using BackgroundImageSorter.Model;
using MoreLinq;
using NDesk.Options;
using System;
using System.Collections.Generic;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BackgroundImageSorter
{
    public class Program
    {
        static void Main(string[] args)
        {
            Report report = new BackgroundImageSorter
                                                .Program()
                                                .RunProgram(args);
            PrintReport(report);
        }

        private static void PrintReport(Report report)
        {
            if (report == null) return;

            Console.WriteLine();

            Console.WriteLine(report);

            Console.WriteLine("Press Any Key to exit...");
            while (!Console.KeyAvailable) { }
        }

        public Report RunProgram(string[] args)
        {
            Configuration config = BuildConfig();

            config = SetupConfiguration(args, config);

            if (!config.Error)
                return CommitPurpose(config, new Model.Report());
            else
                return null;
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
                DisplayErrorMessage(e);
                config.Error = true;
            }

            SetDefaultDirectories(config);
            ConfirmImportantFoldersExist(config);

            return config;
        }

        private static void ConfirmImportantFoldersExist(Configuration config)
        {
            if (!config.Destination.Exists || !config.Source.Exists)
            {
                DisplayError();
                config.Error = true;
            }
        }

        private static void ConsiderHelp(Configuration config, OptionSet options)
        {
            if (config.ShowHelp)
            {
                ShowHelp(options);
                config.Error = true;
            }
        }

        private static Report DisplayError()
        {
            Console.WriteLine("An important directory is missing.");
            return null;
        }

        public static Report CommitPurpose(Configuration config, Report report)
        {
            PhotoDao photoDao = LoadData(config, report);

            IEnumerable<Photo> uniquePhotos = ScanSource(config, report, photoDao);

            CopyFiles(config, report, uniquePhotos);

            UpdateData(config, report, photoDao);

            return ProcessReport(config, report);
        }

        private static Report ProcessReport(Configuration config, Report report)
        {
            if (config.SuppressReport) return null;

            return report;
        }

        private static void UpdateData(Configuration config, Report report, PhotoDao photoDao)
        {
            Console.Write("Updating File Data...");

            if (!config.NoUpdate)
                updateDirectoryData(config, photoDao);

            UpdateReportWithNewImageCount(config, report);

            Console.WriteLine("Dao updated.");
        }

        private static void CopyFiles(Configuration config, Report report, IEnumerable<Photo> uniquePhotos)
        {
            Console.Write("Copying Photos...");

            moveUniquePhotosToAppropiateDirectory(uniquePhotos,
                                                    config, report);
            Console.WriteLine("Copy Complete.");
        }

        private static IEnumerable<Photo> ScanSource(Configuration config, Report report, PhotoDao photoDao)
        {
            Console.Write("Scaning Source Directory...");
            IEnumerable<Photo> uniquePhotos = scanDirectoryForNewPhotos(config.Source, photoDao, report);

            Console.WriteLine("Complete.");
            return uniquePhotos;
        }

        private static PhotoDao LoadData(Configuration config, Report report)
        {
            Console.Write("Dao loading...");

            PhotoDao photoDao = new PhotoDao(config.DataFile.FullName);

            LoadDaoInfoToReport(report, photoDao);

            Console.WriteLine("Complete.");
            return photoDao;
        }

        private static void UpdateReportWithNewImageCount(Configuration config, Report report)
        {
            report.ImagesInLandscapeFolder = config.Landscape
                                        .GetFiles()
                                        .Count();
        }

        private static void LoadDaoInfoToReport(Report report, PhotoDao photoDao)
        {
            report.StoredFiles = photoDao.size();
            report.StoredImages = photoDao.Images();
            report.StoredBackgrounds = photoDao.Backgrounds();
        }

        public static Configuration SetDefaultDirectories(Configuration config)
        {
            if (config.Source == null)
                config.Source = new DirectoryInfo(@"C:\Users\" + Environment.UserName + @"\AppData\Local\Packages\Microsoft.Windows.ContentDeliveryManager_cw5n1h2txyewy\LocalState\Assets");

            if (config.Destination == null)
                config.Destination = new DirectoryInfo(@"C:\Users\" + Environment.UserName + @"\Pictures\Saved Pictures");

            if (config.DataFile == null)
                config.DataFile = new FileInfo(@".\PhotoData.bin");

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

        private static Report DisplayErrorMessage(OptionException e)
        {
            Console.Write("BackgroundImageSorter: ");
            Console.WriteLine(e.Message);
            Console.WriteLine("Try `BackgroundImageSorter --help' for more information.");
            return null;
        }

        private static void ParseArguments(string[] args, OptionSet p)
        {
            List<string> extra = p.Parse(args);
            if (extra.Count > 0) throw new OptionException(extra.First<string>(), "Unidentified Option");
        }

        private static OptionSet SetOptions(Configuration config)
        {
            return new NDesk.Options.OptionSet() {
                { "d|DataFile=", "Dao Data File", d => config.DataFile = new FileInfo(d) },
                { "s|Source=", "Folder to Pull Images From", v => config.Source = new DirectoryInfo(v) },
                { "o|Output=",  "Folder to Place Sorted Images Into", v => config.Destination = new DirectoryInfo(v)},
                { "p|Portrait=",  "Folder to Place Portrait Images Into", v => config.Portrait = new DirectoryInfo(v)},
                { "l|Landscape=",  "Folder to Place Landscape Images Into", v => config.Landscape = new DirectoryInfo(v)},
                { "f|Fast", "Fast Scan - Use File Names Instead of Hashes.", v => config.FastScan = v != null },
                { "b|Background", "Only Copy Images Larger Than 1080x1080.", v => config.LargeImagesOnly = v != null },
                { "i|Image", "Only Copy Images - Unrecognized Files Ignored", v => config.ImagesOnly = v != null },
                { "t|Test", "Print Files Copied, But Do No Actual Copying.", v => config.Test = v != null },
                { "a|Automated", "Suppress the Report and Do Not Pause at the End\n\tUseful For Batch Scripts.", v => config.SuppressReport = v != null },
                { "h|help",  "show this message and exit",
                        v => config.ShowHelp = v != null },
                { "?",  "show this message and exit",
                        v => config.ShowHelp = v != null }
            };
        }

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
                SuppressReport = false
            };
        }

        private static void updateDirectoryData(Configuration config,
                                                    PhotoDao photoDao)
        {

            ISet<DirectoryInfo> directories = new HashSet<DirectoryInfo>();
            directories.Add(config.Portrait);
            directories.Add(config.Landscape);
            directories.Add(config.DataDirectory);
            directories.Add(config.BackgroundDirectory);
            directories.Add(config.Destination);
            directories.Add(config.SmallDirectory);

            if (config.BackgroundDirectory.Exists)
                config.BackgroundDirectory?
                    .GetDirectories()
                    .ToList<DirectoryInfo>()
                    .ForEach(dir => directories.Add(dir));

            directories.ToList<DirectoryInfo>().ForEach(directory => updateDirectoryData(directory, photoDao));
        }

        private static void updateDirectoryData(DirectoryInfo directory, PhotoDao photoDao)
        {
            if (directory.Exists)
                Array.ForEach<FileInfo>(directory.GetFiles(), file => photoDao.Create(PhotoBuilder.Build(file.FullName)));
        }


        private static void moveUniquePhotosToAppropiateDirectory(IEnumerable<Photo> uniquePhotos,
                                        Configuration config,
                                        Report report)
        {


            foreach (Photo photo in uniquePhotos)
            {
                System.Drawing.Size dimension = photo.Dimension;
                try
                {
                    if (dimension.IsEmpty && !config.ImagesOnly)
                    {
                        CreateNonExistantDirectory(config.DataDirectory);

                        if (!config.Test)
                            photo.FileInfo.CopyTo(config.DataDirectory.FullName + @"\" + photo.FileInfo.Name, false);

                    }
                    else if (dimension.Height >= 1080 && dimension.Width >= 1080)
                    {
                        //CreateNonExistantDirectory(config.BackgroundDirectory);

                        if (dimension.Height > dimension.Width)
                        {
                            CreateNonExistantDirectory(config.Portrait);

                            if (!config.Test)
                                photo.FileInfo.CopyTo(config.Portrait.FullName + @"\" + photo.FileInfo.Name + ".jpg", false);
                        }
                        else
                        {
                            CreateNonExistantDirectory(config.Landscape);

                            if (!config.Test)
                                photo.FileInfo.CopyTo(config.Landscape.FullName + @"\" + photo.FileInfo.Name + ".jpg", false);
                        }
                        report.Moved++;
                    }
                    else
                    {
                        if (!config.LargeImagesOnly)
                        {
                            CreateNonExistantDirectory(config.SmallDirectory);

                            if (!config.Test)
                                photo.FileInfo.CopyTo(config.SmallDirectory.FullName + @"\" + photo.FileInfo.Name + ".png", false);
                        }
                    }
                }
                catch (System.IO.IOException ex)
                {
                    Console.WriteLine("Skipping " + photo.FileInfo.Name);
                    Console.WriteLine(ex.Message);
                }
            }
        }

        private static void CreateNonExistantDirectory(DirectoryInfo directory)
        {
            if (!directory.Exists)
                directory.Create();
        }

        private static IEnumerable<Photo> scanDirectoryForNewPhotos(DirectoryInfo sourceDirectory, PhotoDao photoDao, Report report)
        {
            FileInfo[] possiblePhotos = sourceDirectory.GetFiles();

            IEnumerable<Photo> photos = possiblePhotos.Select(possiblePhoto => PhotoBuilder.Build(possiblePhoto.FullName));

            report.Scanned = photos.Count();

            IEnumerable<Photo> filteredPhotos = photos.Where<Photo>(photo => !photoDao.Contains(photo));

            filteredPhotos = GetDistinctPhotos(filteredPhotos);

            report.Skipped = report.Scanned - filteredPhotos.Count();

            return filteredPhotos;

        }

        public static IEnumerable<Photo> GetDistinctPhotos(IEnumerable<Photo> filteredPhotos)
        {
            return getDistinctPhotos(filteredPhotos);
        }

        private static IEnumerable<Photo> getDistinctPhotos(IEnumerable<Photo> photos)
        {

            var hashes = photos.Select(photo => photo.SHA512);

            List<Photo> uniques = new List<Photo>();

            foreach (Photo photo in photos)
            {
                byte[] hash = photo.SHA512;
                byte[] validOrNull = uniques.Select(testPhoto => testPhoto.SHA512).Where(testHash => testHash.SequenceEqual(hash)).SingleOrDefault();
                if (validOrNull == null)
                    uniques.Add(photo);
            }

            return uniques;
        }

        static void ShowHelp(OptionSet p)
        {
            Console.WriteLine("Usage: [OPTIONS]+");
            Console.WriteLine();
            Console.WriteLine("Options:");
            p.WriteOptionDescriptions(Console.Out);
            Console.WriteLine();
            Console.WriteLine("\t Fast Scan Not Implemented Yet.");
        }
    }
}
