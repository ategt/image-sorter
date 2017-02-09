using BackgroundImageSorter.Model;
using BackgroundImageSorter.View;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BackgroundImageSorter.Controller
{
    public class IOController
    {
        ConsoleView consoleView;

        public IOController(ConsoleView consoleView)
        {
            this.consoleView = consoleView;
        }

        public void updateDirectoryData(Configuration config,
                                    PhotoDao photoDao)
        {
            ISet<DirectoryInfo> directories = GenerateScannableDirectoriesSet(config);

            List<DirectoryInfo> directoriesToScan = directories.ToList<DirectoryInfo>();
            directoriesToScan.RemoveAll(item => item == null);

            List<FileInfo> filesToScanList = new List<FileInfo>();
            //FileInfo[] filesToScanList = new FileInfo[]();
            directoriesToScan.ForEach(directory =>
            {
                FileInfo[] filesToScan = updateDirectoryData(directory, photoDao);
                if (filesToScan != null)
                {
                    int i = 0;
                    Console.WriteLine($"Scan list length: {filesToScan.Length}");
                    foreach (FileInfo file in filesToScan)
                    {                        
                        Console.WriteLine($"Scanning {file.Name}:{i++}");
                        //ApplicationController.AddPhotoToDao(photoDao, file);
                        filesToScanList.Add(file);
                    }
                }
            });

            Console.WriteLine($"List length: {filesToScanList.Count}");
            foreach (FileInfo file in filesToScanList)
            {
                ApplicationController.AddPhotoToDao(photoDao, file);
                Console.WriteLine($"Listed {file.Name}");
            }
        }

        private static ISet<DirectoryInfo> GenerateScannableDirectoriesSet(Configuration config)
        {
            ISet<DirectoryInfo> directories = new HashSet<DirectoryInfo>();
            directories.Add(config.Portrait);
            directories.Add(config.Landscape);
            directories.Add(config.DataDirectory);
            directories.Add(config.BackgroundDirectory);
            directories.Add(config.Destination);
            directories.Add(config.SmallDirectory);

            if (config.BackgroundDirectory?.Exists ?? false)
                config.BackgroundDirectory?
                    .GetDirectories()
                    .ToList<DirectoryInfo>()
                    .ForEach(dir => directories.Add(dir));
            return directories;
        }

        private static FileInfo[] updateDirectoryData(DirectoryInfo directory, PhotoDao photoDao)
        {
            directory?.Refresh();

            if (directory == null)
            {
                return null;
            }
            else if (directory.Exists)
            {
                //Array.ForEach<FileInfo>(directory.GetFiles(), file => ApplicationController.AddPhotoToDao(photoDao, file));
                return directory.GetFiles();
            }
            else
            {
                ConsoleView.DisplayDirectoryDoesNotExist(directory);
                return null;
            }
        }

    }
}
