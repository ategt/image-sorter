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
            List<FileInfo> filesToScan = getFilesToScan(config);

            scanFilesInListToDao(photoDao, filesToScan);
        }

        private void scanFilesInListToDao(PhotoDao photoDao, List<FileInfo> filesToScan)
        {
            int i = 0, total = filesToScan.Count;
            foreach (FileInfo file in filesToScan)
            {
                consoleView.DisplayDaoScanProgress(i++, total);
                ApplicationController.AddPhotoToDao(photoDao, file);
            }
        }

        private List<FileInfo> scanDirectoriesForFiles(List<DirectoryInfo> directoriesToScan)
        {
            List<FileInfo> filesToScanList = new List<FileInfo>();
            directoriesToScan.ForEach(directory =>
            {
                FileInfo[] filesToScan = getFilesFromValidDirectories(directory);
                addFilesToFilesList(filesToScanList, filesToScan);
            });
            return filesToScanList;
        }

        private void addFilesToFilesList(List<FileInfo> filesToScanList, FileInfo[] filesToScan)
        {
            if (filesToScan != null)
            {
                foreach (FileInfo file in filesToScan)
                {
                    filesToScanList.Add(file);
                }
            }
        }

        private List<DirectoryInfo> RemoveNullDirectories(ISet<DirectoryInfo> directories)
        {
            List<DirectoryInfo> directoriesToScan = directories.ToList<DirectoryInfo>();
            directoriesToScan.RemoveAll(item => item == null);
            return directoriesToScan;
        }

        private ISet<DirectoryInfo> GenerateScannableDirectoriesSet(Configuration config)
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

        internal void updateDirectoryDataFast(Configuration config, PhotoDao photoDao)
        {
            List<FileInfo> filesToScan = getFilesToScan(config);
            filesToScan = removeFilesAlreadyInDao(filesToScan, photoDao);

            scanFilesInListToDao(photoDao, filesToScan);
        }

        private List<FileInfo> removeFilesAlreadyInDao(List<FileInfo> filesToScan, PhotoDao photoDao)
        {
            return filesToScan.Where(file => !photoDao.ContainsFile(file)).ToList();
        }

        private List<FileInfo> getFilesToScan(Configuration config)
        {
            ISet<DirectoryInfo> directories = GenerateScannableDirectoriesSet(config);

            List<DirectoryInfo> directoriesToScan = RemoveNullDirectories(directories);

            List<FileInfo> filesToScan = scanDirectoriesForFiles(directoriesToScan);
            return filesToScan;
        }

        private FileInfo[] getFilesFromValidDirectories(DirectoryInfo directory)
        {
            directory?.Refresh();

            if (directory == null)
            {
                return null;
            }
            else if (directory.Exists)
            {
                return directory.GetFiles();
            }
            else
            {
                consoleView.DisplayDirectoryDoesNotExist(directory);
                return null;
            }
        }

    }
}
