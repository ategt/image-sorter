using BackgroundImageSorter;
using BackgroundImageSorter.Model;
using MoreLinq;
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

            Configuration config = new Configuration();

            var p = new NDesk.Options.OptionSet() {
                { "d|DataFile=", "Dao Data File", d => config.DataFile = new FileInfo(d) },
                { "s|Source=", "Folder to Pull Images From", v => config.Source = new DirectoryInfo(v) },
                { "o|Output=",  "Folder to Place Sorted Images Into", v => config.Destination = new DirectoryInfo(v)},
                { "p|Portrait=",  "Folder to Place Portrait Images Into", v => config.Portrait = new DirectoryInfo(v)},
                { "l|Landscape=",  "Folder to Place Landscape Images Into", v => config.Landscape = new DirectoryInfo(v)},
                { "f|Fast", "Fast Scan - Use File Names Instead of Hashes.", v => config.FastScan = v != null },
                { "b|Background", "Only Copy Images Larger Than 1080x1080.", v => config.LargeImagesOnly = v != null },
                { "i|Image", "Only Copy Images - Unrecognized Files Ignored", v => config.ImagesOnly = v != null },
                { "h|help",  "show this message and exit",
                        v => config.ShowHelp = v != null },
                { "?",  "show this message and exit",
                        v => config.ShowHelp = v != null }
            };

            List<string> extra = p.Parse(args);

            if (config.Source == null)
                config.Source = new DirectoryInfo(@"C:\Users\" + Environment.UserName + @"\AppData\Local\Packages\Microsoft.Windows.ContentDeliveryManager_cw5n1h2txyewy\LocalState\Assets");

            if (config.Destination == null)
                config.Destination = new DirectoryInfo(@"C:\Users\" + Environment.UserName + @"\Pictures\Saved Pictures");

            if (config.DataFile == null)
                config.DataFile = new FileInfo(@".\PhotoData.bin");

            //Configuration config = new Configuration();

            //string source = @"C:\Users\ATeg\Desktop\tests\input";
            //string destination = @"C:\Users\ATeg\Desktop\tests\output";

            //string source = @"C:\Users\" + Environment.UserName + @"\AppData\Local\Packages\Microsoft.Windows.ContentDeliveryManager_cw5n1h2txyewy\LocalState\Assets";
            //string source = @"C:\Users\ATeg\Desktop\imgTemp";
            //DirectoryInfo sourceDirectory = new DirectoryInfo(source);
            //DirectoryInfo primaryDirectory = new DirectoryInfo(@"C:\Users\ATeg\Desktop\Screenshots\Images");
            //DirectoryInfo primaryDirectory = new DirectoryInfo(".");
            //DirectoryInfo primaryDirectory = new DirectoryInfo(destination);

            //ImageFormat.

            Report report = new Report();

            if (config.Destination.Exists)
            {
                //if (config.Portrait == null && config.Landscape == null)


                //DirectoryInfo dataDirectory = primaryDirectory.CreateSubdirectory("Data");
                //DirectoryInfo smallDirectory = primaryDirectory.CreateSubdirectory("Other Images");


                //PhotoDao photoDao = new PhotoDao(@"PhotoFile-Test.bin");
                PhotoDao photoDao = new PhotoDao(config.DataFile.FullName);

                report.StoredFiles = photoDao.size();
                report.StoredImages = photoDao.Images();
                report.StoredBackgrounds = photoDao.Backgrounds();

                Console.WriteLine("Dao loaded.");

                IEnumerable<Photo> uniquePhotos = scanDirectoryForNewPhotos(config.Source, photoDao, report);

                Console.WriteLine("Scan Complete.");

                moveUniquePhotosToAppropiateDirectory(uniquePhotos,
                                                        config, report);
                Console.WriteLine("Copy Complete.");

                updateDirectoryData(config, photoDao);
                report.ImagesInLandscapeFolder = config.Landscape
                                            .GetFiles()
                                            .Count();

                Console.WriteLine("Dao data updated.");

                Console.WriteLine();

                Console.WriteLine(report);

                Console.WriteLine("Press Any Key to exit...");
                while (!Console.KeyAvailable) { }
            }
            else
            {
                Console.WriteLine("An important directory is missing.");
            }



        }

        private static void updateDirectoryData(Configuration config,
                                                    PhotoDao photoDao)
        {

            IList<DirectoryInfo> directories = new List<DirectoryInfo>();
            directories.Add(config.Portrait);
            directories.Add(config.Landscape);
            directories.Add(config.DataDirectory);
            directories.Add(config.BackgroundDirectory);
            directories.Add(config.Destination);
            directories.Add(config.SmallDirectory);

            config.BackgroundDirectory
                .GetDirectories()
                .ToList<DirectoryInfo>()
                .ForEach(dir => directories.Add(dir));

            directories.ToList<DirectoryInfo>().ForEach(directory => updateDirectoryData(directory, photoDao));
        }

        private static void updateDirectoryData(DirectoryInfo directory, PhotoDao photoDao)
        {
            Array.ForEach<FileInfo>(directory.GetFiles(), file => photoDao.Create(PhotoBuilder.Build(file.FullName)));
        }


        private static void moveUniquePhotosToAppropiateDirectory(IEnumerable<Photo> uniquePhotos,
                                        //DirectoryInfo backgroundDirectory,
                                        //DirectoryInfo smallDirectory,
                                        //DirectoryInfo dataDirectory,
                                        Configuration config,
                                        Report report)
        {


            foreach (Photo photo in uniquePhotos)
            {
                System.Drawing.Size dimension = photo.Dimension;
                try
                {
                    if (dimension.IsEmpty)
                    {
                        if (config.DataDirectory == null)
                        {
                            config.DataDirectory = config.Destination.CreateSubdirectory("Data");
                        }

                        photo.FileInfo.CopyTo(config.DataDirectory.FullName + @"\" + photo.FileInfo.Name, false);

                    }
                    else if (dimension.Height >= 1080 && dimension.Width >= 1080)
                    {
                        if (config.BackgroundDirectory == null)
                        {
                            config.BackgroundDirectory = config.Destination.CreateSubdirectory("Backgrounds");
                        }

                        if (dimension.Height > dimension.Width)
                        {
                            if (config.Portrait == null)
                                config.Portrait = config.BackgroundDirectory.CreateSubdirectory("Portrait");
                            photo.FileInfo.CopyTo(config.Portrait.FullName + @"\" + photo.FileInfo.Name + ".jpg", false);
                        }
                        else
                        {
                            if (config.Landscape == null)
                                config.Landscape = config.BackgroundDirectory.CreateSubdirectory("Landscape");
                            photo.FileInfo.CopyTo(config.Landscape.FullName + @"\" + photo.FileInfo.Name + ".jpg", false);
                        }
                        report.Moved++;
                    }
                    else
                    {
                        if (config.SmallDirectory == null)
                            config.SmallDirectory = config.Destination.CreateSubdirectory("Other Images");

                        photo.FileInfo.CopyTo(config.SmallDirectory.FullName + @"\" + photo.FileInfo.Name + ".png", false);
                    }
                }
                catch (System.IO.IOException ex)
                {
                    Console.WriteLine("Skipping " + photo.FileInfo.Name);
                    Console.WriteLine(ex.Message);
                }
            }
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
            //filteredPhotos = filteredPhotos.DistinctBy(photo => new { photo.SHA512, photo.SHA256, photo.Digest });
            //return filteredPhotos.DistinctBy(photo => photo.Digest );
            //filteredPhotos = filteredPhotos.DistinctBy(photo => photo.SHA512);
            //filteredPhotos = filteredPhotos.Distinct<Photo>(new EqualityComparer<Photo>
            //DistictBy<IEnumerable<Photo>, byte[]>(filteredPhotos
            //DistictBy
            //filteredPhotos = filteredPhotos.ToList().D
            //return filteredPhotos;

            return getDistinctPhotos(filteredPhotos);

        }

        private static IEnumerable<Photo> getDistinctPhotos(IEnumerable<Photo> photos)
        {

            var hashes = photos.Select(photo => photo.SHA512);
            //var dhash = hashes.Distinct();

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

    }
}
