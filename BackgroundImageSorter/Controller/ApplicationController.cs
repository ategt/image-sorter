using BackgroundImageSorter.Model;
using BackgroundImageSorter.Utilities;
using BackgroundImageSorter.View;
using NDesk.Options;
using System;
using System.Collections.Generic;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;


namespace BackgroundImageSorter.Controller
{
    public class ApplicationController
    {
        IOController ioController = null;
        ConsoleView consoleView = null;

        public ApplicationController(IOController ioController, ConsoleView consoleView)
        {
            this.ioController = ioController;
            this.consoleView = consoleView;
        }

        public static void Program(string[] args, IOController ioController, ConsoleView consoleView)
        {
            Report report = new BackgroundImageSorter
                                                .Controller
                                                .ApplicationController(ioController, consoleView)
                                                .RunProgram(args);
            ConsoleView.PrintReport(report);
        }
        
        public Report RunProgram(string[] args)
        {
            Configuration config = ConfigurationController.EstablishConfiguration(args);

            if (!config.Error)
                return SortImages(config, new Model.Report());
            else
                return null;
        }
        
        public Report SortImages(Configuration config, Report report)
        {
            PhotoDao photoDao = LoadData(config, report);

            if (config.PreScan || photoDao.size() < 1)
                PreScanDestination(config, report, photoDao);

            IEnumerable<Photo> uniquePhotos = ScanSource(config, report, photoDao);

            CopyFiles(config, report, uniquePhotos);

            UpdateData(config, report, photoDao);

            return ProcessReport(config, report);
        }

        private void PreScanDestination(Configuration config, Report report, PhotoDao photoDao)
        {
            ConsoleView.DisplayBeginingDirectoryPrescan();

            ioController.updateDirectoryData(config, photoDao);

            ConsoleView.DisplayFinishedDirectoryPrescan();
        }

        private static Report ProcessReport(Configuration config, Report report)
        {
            if (config.SuppressReport) return null;

            return report;
        }

        private void UpdateData(Configuration config, Report report, PhotoDao photoDao)
        {
            ConsoleView.DisplayBeginUpdatingDao();

            if (!config.NoUpdate)
                ioController.updateDirectoryData(config, photoDao);

            UpdateReportWithNewImageCount(config, report, photoDao);

            ConsoleView.DisplayFinishedUpdateingDao();
        }


        private static void CopyFiles(Configuration config, Report report, IEnumerable<Photo> uniquePhotos)
        {
            ConsoleView.DisplayBeginTransferingData();

            moveUniquePhotosToAppropiateDirectory(uniquePhotos,
                                                    config, report);
            ConsoleView.DisplayFinishedTransferingData();
        }

        private static IEnumerable<Photo> ScanSource(Configuration config, Report report, PhotoDao photoDao)
        {
            ConsoleView.DisplaySourceScanningBegining();
            IEnumerable<Photo> uniquePhotos = scanDirectoryForNewPhotos(config.Source, photoDao, report, config.Recurse);

            ConsoleView.DisplaySourceScanningFinished();
            return uniquePhotos;
        }

        private static PhotoDao LoadData(Configuration config, Report report)
        {
            ConsoleView.DisplayDaoLoadingBeginning();

            PhotoDao photoDao = new PhotoDao(config.DataFile.FullName);

            LoadDaoInfoToReport(report, photoDao);

            ConsoleView.DisplayDaoLoadingFinished();
            return photoDao;
        }


        private static void UpdateReportWithNewImageCount(Configuration config, Report report, PhotoDao photoDao)
        {
            report.ImagesInLandscapeFolder = config.Landscape
                                        .GetFiles()
                                        .Count();

            report.PostImageCount = photoDao.size();
        }

        private static void LoadDaoInfoToReport(Report report, PhotoDao photoDao)
        {
            report.StoredFiles = photoDao.size();
            report.StoredImages = photoDao.Images();
            report.StoredBackgrounds = photoDao.Backgrounds();
        }



        public static Photo AddPhotoToDao(PhotoDao photoDao, FileInfo file)
        {
            string filePath = file.FullName;
            Photo photo = PhotoBuilder.Build(filePath);
            Photo returnedPhoto = photoDao.Create(photo);

            return returnedPhoto;
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
                    CopyFilesToAppropriateDirectory(config, report, photo, dimension);
                }
                catch (System.IO.IOException ex)
                {
                    ConsoleView.DisplaySkippingPhotoDuringTransfer(photo, ex);
                }
            }
        }

        private static void CopyFilesToAppropriateDirectory(Configuration config, Report report, Photo photo, System.Drawing.Size dimension)
        {
            if (dimension.IsEmpty && !config.ImagesOnly)
            {
                CopyToDirectory(config, report, photo, config.DataDirectory);
            }
            else if (dimension.Height >= 1080 && dimension.Width >= 1080)
            {
                CopyLargeImagesToApproprateDirectory(config, report, photo, dimension);
            }
            else
            {
                CopyOtherImages(config, report, photo);
            }
        }

        private static void CopyOtherImages(Configuration config, Report report, Photo photo)
        {
            if (!config.LargeImagesOnly)
            {
                CopyToDirectory(config, report, photo, config.SmallDirectory);
            }
        }

        private static void CopyLargeImagesToApproprateDirectory(Configuration config, Report report, Photo photo, System.Drawing.Size dimension)
        {
            if (dimension.Height > dimension.Width)
            {
                CopyToDirectory(config, report, photo, config.Portrait);
            }
            else
            {
                CopyToDirectory(config, report, photo, config.Landscape);
            }
        }

        private static void CopyToDirectory(Configuration config, Report report, Photo photo, DirectoryInfo destinationDirectory)
        {
            if (!config.Test)
            {
                CreateNonExistantDirectory(destinationDirectory);
                photo.FileInfo.CopyTo(GenerateNewFullName(config, photo, destinationDirectory), false);
            }
            report.Moved++;
        }

        private static string GenerateNewFullName(Configuration config, Photo photo, DirectoryInfo destinationDirectory)
        {
            if (config?.RebuildExtensions ?? false)
                return destinationDirectory.FullName + @"\" + RebuildImageExtension(photo, config.AggressiveExtensions);
            else
                return destinationDirectory.FullName + @"\" + photo.FileInfo.Name;
        }

        public static string RebuildImageExtension(Photo photo, bool aggressive)
        {
            string properExtension = string.Empty;
            Guid format = photo.Format;
            if (format.Equals(ImageFormat.Jpeg.Guid))
            {
                properExtension = "jpg";
            }
            else if (format.Equals(ImageFormat.Bmp.Guid))
            {
                properExtension = "bmp";
            }
            else if (format.Equals(ImageFormat.Exif.Guid))
            {
                properExtension = "exif";
            }
            else if (format.Equals(ImageFormat.Emf.Guid))
            {
                properExtension = "emf";
            }
            else if (format.Equals(ImageFormat.Gif.Guid))
            {
                properExtension = "gif";
            }
            else if (format.Equals(ImageFormat.Icon.Guid))
            {
                properExtension = "icon";
            }
            else if (format.Equals(ImageFormat.MemoryBmp.Guid))
            {
                properExtension = "mbmp";
            }
            else if (format.Equals(ImageFormat.Png.Guid))
            {
                properExtension = "png";
            }
            else if (format.Equals(ImageFormat.Tiff.Guid))
            {
                properExtension = "tiff";
            }
            else if (format.Equals(ImageFormat.Wmf.Guid))
            {
                properExtension = "wmf";
            }
            else
            {
                properExtension = string.Empty;
            }

            if (string.IsNullOrWhiteSpace(properExtension))
                return photo.FileInfo.Name;
            else
                return RemoveExtensions(photo, aggressive) + "." + properExtension;

        }

        private static string RemoveExtensions(Photo photo, bool aggresive)
        {
            if (aggresive)
            {
                string name = photo.FileInfo.Name;
                return name.Split(new char[] { '.' })[0];
            }
            else
            {
                return Path.GetFileNameWithoutExtension(photo.FileInfo.FullName);
            }
        }

        private static void CreateNonExistantDirectory(DirectoryInfo directory)
        {
            if (!directory.Exists)
            {
                directory.Create();
                directory.Refresh();
            }
        }

        private static IEnumerable<Photo> scanDirectoryForNewPhotos(DirectoryInfo sourceDirectory, PhotoDao photoDao, Report report, bool recurse)
        {
            FileInfo[] possiblePhotos = (recurse) ?
                                sourceDirectory.GetFiles("*", SearchOption.AllDirectories) :
                                        sourceDirectory.GetFiles("*", SearchOption.TopDirectoryOnly);

            IEnumerable<Photo> photos = possiblePhotos.Select(possiblePhoto => PhotoBuilder.Build(possiblePhoto.FullName));

            report.Scanned = photos.Count();

            IEnumerable<Photo> filteredPhotos = filterOutTheFilesWeAlreadyHave(photoDao, photos);

            report.NewPhotos = filteredPhotos.Count();
            report.AlreadyHad = report.Scanned - filteredPhotos.Count();

            filteredPhotos = GetDistinctPhotos(filteredPhotos);

            report.Distinct = filteredPhotos.Count();

            return filteredPhotos;
        }

        private static IEnumerable<Photo> filterOutTheFilesWeAlreadyHave(PhotoDao photoDao, IEnumerable<Photo> photos)
        {

            List<Photo> photosWeDoNotHaveYet = new List<Photo>();

            foreach (var photo in photos)
            {
                bool alreadyHaveThisOne = photoDao.Contains(photo);

                if (!alreadyHaveThisOne)
                {
                    photosWeDoNotHaveYet.Add(photo);
                }
                else
                {
                    ConsoleView.DisplayAFileHasBeenFilteredOut();
                }
            }

            return photosWeDoNotHaveYet;
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


    }
}
