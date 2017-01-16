﻿using BackgroundImageSorter;
using BackgroundImageSorter.Model;
using MoreLinq;
using System;
using System.Collections.Generic;
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
            //string source = @"C:\Users\ATeg\Desktop\tests\input";
            string destination = @"C:\Users\ATeg\Desktop\tests\output";

            string source = @"C:\Users\" + Environment.UserName + @"\AppData\Local\Packages\Microsoft.Windows.ContentDeliveryManager_cw5n1h2txyewy\LocalState\Assets";
            //string source = @"C:\Users\ATeg\Desktop\imgTemp";
            DirectoryInfo sourceDirectory = new DirectoryInfo(source);
            //DirectoryInfo primaryDirectory = new DirectoryInfo(@"C:\Users\ATeg\Desktop\Screenshots\Images");
            //DirectoryInfo primaryDirectory = new DirectoryInfo(".");
            DirectoryInfo primaryDirectory = new DirectoryInfo(destination);

            Report report = new Report();

            if (primaryDirectory.Exists)
            {
                DirectoryInfo backgroundDirectory = primaryDirectory.CreateSubdirectory("Backgrounds");
                DirectoryInfo dataDirectory = primaryDirectory.CreateSubdirectory("Data");
                DirectoryInfo smallDirectory = primaryDirectory.CreateSubdirectory("Other Images");

                PhotoDao photoDao = new PhotoDao(@"PhotoFile-Test.bin");

                report.StoredFiles = photoDao.size();
                report.StoredImages = photoDao.Images();
                report.StoredBackgrounds = photoDao.Backgrounds();

                Console.WriteLine("Dao loaded.");

                IEnumerable<Photo> uniquePhotos = scanDirectoryForNewPhotos(sourceDirectory, photoDao, report);

                Console.WriteLine("Scan Complete.");

                moveUniquePhotosToAppropiateDirectory(uniquePhotos,
                                                        backgroundDirectory,
                                                        smallDirectory,
                                                        dataDirectory, report);
                Console.WriteLine("Copy Complete.");

                updateDirectoryData(backgroundDirectory,
                                                        smallDirectory,
                                                        dataDirectory,
                                                        photoDao);
                report.Images = backgroundDirectory
                                            .GetDirectories()
                                            .FirstOrDefault()
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

        private static void updateDirectoryData(DirectoryInfo backgroundDirectory,
                                                    DirectoryInfo smallDirectory,
                                                    DirectoryInfo dataDirectory,
                                                    PhotoDao photoDao)
        {

            IList<DirectoryInfo> directories = new List<DirectoryInfo>();
            directories.Add(backgroundDirectory);
            directories.Add(smallDirectory);
            directories.Add(dataDirectory);

            backgroundDirectory.GetDirectories()
                .ToList<DirectoryInfo>()
                .ForEach(dir => directories.Add(dir));

            directories.ToList<DirectoryInfo>().ForEach(directory => updateDirectoryData(directory, photoDao));
        }

        private static void updateDirectoryData(DirectoryInfo directory, PhotoDao photoDao)
        {
            Array.ForEach<FileInfo>(directory.GetFiles(), file => photoDao.Create(PhotoBuilder.Build(file.FullName)));
        }


        private static void moveUniquePhotosToAppropiateDirectory(IEnumerable<Photo> uniquePhotos,
                                        DirectoryInfo backgroundDirectory,
                                        DirectoryInfo smallDirectory,
                                        DirectoryInfo dataDirectory,
                                        Report report)
        {
            DirectoryInfo portraitDirectory = backgroundDirectory.CreateSubdirectory("Portrait");
            DirectoryInfo landscapeDirectory = backgroundDirectory.CreateSubdirectory("Landscape");

            foreach (Photo photo in uniquePhotos)
            {
                System.Drawing.Size dimension = photo.Dimension;
                try
                {
                    if (dimension.IsEmpty)
                    {

                        photo.FileInfo.CopyTo(dataDirectory.FullName + @"\" + photo.FileInfo.Name, false);

                    }
                    else if (dimension.Height >= 1080 && dimension.Width >= 1080)
                    {
                        if (dimension.Height > dimension.Width)
                        {
                            photo.FileInfo.CopyTo(portraitDirectory.FullName + @"\" + photo.FileInfo.Name + ".jpg", false);
                        }
                        else
                        {
                            photo.FileInfo.CopyTo(landscapeDirectory.FullName + @"\" + photo.FileInfo.Name + ".jpg", false);
                        }
                        report.Moved++;
                    }
                    else
                    {
                        photo.FileInfo.CopyTo(smallDirectory.FullName + @"\" + photo.FileInfo.Name + ".png", false);
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
