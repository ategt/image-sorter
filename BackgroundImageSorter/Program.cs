using BackgroundImageSorter.Model;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BackgroundImageSorter
{
    class Program
    {
        static void Main(string[] args)
        {

            string source = @"C:\Users\" + Environment.UserName + @"\AppData\Local\Packages\Microsoft.Windows.ContentDeliveryManager_cw5n1h2txyewy\LocalState\Assets";
            DirectoryInfo sourceDirectory = new DirectoryInfo(source);
            DirectoryInfo primaryDirectory = new DirectoryInfo(@"C:\Users\ATeg\Desktop\test_images");

            if (primaryDirectory.Exists)
            {
                DirectoryInfo backgroundDirectory = primaryDirectory.CreateSubdirectory("Backgrounds");
                DirectoryInfo dataDirectory = primaryDirectory.CreateSubdirectory("Data");
                DirectoryInfo smallDirectory = primaryDirectory.CreateSubdirectory("Other Images");

                PhotoDao photoDao = new PhotoDao();
                Console.WriteLine("Dao loaded.");

                IEnumerable<Photo> uniquePhotos = scanDirectoryForNewPhotos(sourceDirectory, photoDao);

                Console.WriteLine("Scan Complete.");

                moveUniquePhotosToAppropiateDirectory(uniquePhotos,
                                                        backgroundDirectory,
                                                        smallDirectory,
                                                        dataDirectory);
                Console.WriteLine("Copy Complete.");

                updateDirectoryData(backgroundDirectory,
                                                        smallDirectory,
                                                        dataDirectory,
                                                        photoDao);
                Console.WriteLine("Dao data updated.");
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
            Array.ForEach<FileInfo>(backgroundDirectory.GetFiles(), file => photoDao.Create(PhotoBuilder.Build(file.FullName)));
            Array.ForEach<FileInfo>(smallDirectory.GetFiles(), file => photoDao.Create(PhotoBuilder.Build(file.FullName)));
            Array.ForEach<FileInfo>(dataDirectory.GetFiles(), file => photoDao.Create(PhotoBuilder.Build(file.FullName)));
        }

        private static void moveUniquePhotosToAppropiateDirectory(IEnumerable<Photo> uniquePhotos,
                                        DirectoryInfo backgroundDirectory,
                                        DirectoryInfo smallDirectory,
                                        DirectoryInfo dataDirectory)
        {
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
                        photo.FileInfo.CopyTo(backgroundDirectory.FullName + @"\" + photo.FileInfo.Name + ".jpg", false);
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

        private static IEnumerable<Photo> scanDirectoryForNewPhotos(DirectoryInfo sourceDirectory, PhotoDao photoDao)
        {
            FileInfo[] possiblePhotos = sourceDirectory.GetFiles();

            Photo[] photos = new Photo[possiblePhotos.Length];

            foreach (FileInfo possiblePhoto in possiblePhotos)
            {
                photos[Array.IndexOf(possiblePhotos, possiblePhoto)] = PhotoBuilder.Build(possiblePhoto.FullName);
            }

            IEnumerable<Photo> filteredPhotos = photos.Where<Photo>(photo => !photoDao.Contains(photo));

            return filteredPhotos;

        }
    }
}
