using BackgroundImageSorter.Model;
using BackgroundImageSorter.View;
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
        IView consoleView = null;
        PhotoDao photoDao = null;
        ConfigurationController configurationController = null;

        public ApplicationController(IOController ioController, IView consoleView, PhotoDao photoDao, ConfigurationController configurationController)
        {
            this.ioController = ioController;
            this.consoleView = consoleView;
            this.photoDao = photoDao;
            this.configurationController = configurationController;
        }

        public static void Program(string[] args, IOController ioController, IView view, PhotoDao photoDao, ConfigurationController configurationController)
        {
            Report report = new BackgroundImageSorter
                                                .Controller
                                                .ApplicationController(ioController, view, photoDao, configurationController)
                                                .RunProgram(args);
            view.PrintReport(report);
        }

        public Report RunProgram(string[] args)
        {
            Configuration config = configurationController.EstablishConfiguration(args);

            if (!config.Error)
                return SortImages(config, new Model.Report());
            else
                return null;
        }

        public Report SortImages(Configuration config, Report report)
        {
            IEnumerable<Photo> uniquePhotos =
                        FindUniquePhotos(config, report);

            CopyOrMoveFiles(config, report, uniquePhotos);

            if (report.Moved > 0)
                UpdateData(config, report);
            else
                consoleView.DisplayNoFilesMoved();

            return ProcessReport(config, report);
        }

        public IEnumerable<Photo> FindUniquePhotos(Configuration config, Report report)
        {
            photoDao = LoadData(config, report);
            if (evaluatePrescanRequirements(config, photoDao))
                PreScanDestination(config, report, photoDao);

            return ScanSource(config, report, photoDao);
        }

        private static bool evaluatePrescanRequirements(Configuration config, PhotoDao photoDao)
        {
            return config.PreScan || photoDao.size() < 1;
        }

        private void PreScanDestination(Configuration config, Report report, PhotoDao photoDao)
        {
            consoleView.DisplayBeginingDirectoryPrescan();

            ioController.updateDirectoryData(config, photoDao);

            consoleView.DisplayFinishedDirectoryPrescan();
        }

        private static Report ProcessReport(Configuration config, Report report)
        {
            if (config.SuppressReport) return null;

            return report;
        }

        private void UpdateData(Configuration config, Report report)
        {
            consoleView.DisplayBeginUpdatingDao();

            if (!config.NoUpdate)
            {
                if (config.FastScan)
                    ioController.updateDirectoryDataFast(config, photoDao);
                else
                {
                    ioController.updateDirectoryData(config, photoDao);
                }
            }

            UpdateReportWithNewImageCount(config, report, photoDao);

            consoleView.DisplayFinishedUpdateingDao();
        }


        private void CopyOrMoveFiles(Configuration config, Report report, IEnumerable<Photo> uniquePhotos)
        {
            consoleView.DisplayBeginTransferingData();
            consoleView.TotalFilesToTransfer = uniquePhotos.Count();

            moveUniquePhotosToAppropiateDirectory(uniquePhotos,
                                                    config, report);
            consoleView.DisplayFinishedTransferingData();
        }

        private IEnumerable<Photo> ScanSource(Configuration config, Report report, PhotoDao photoDao)
        {
            consoleView.DisplaySourceScanningBegining();
            IEnumerable<Photo> uniquePhotos = scanDirectoryForNewPhotos(config.Source, photoDao, report, config.Recurse);

            consoleView.DisplaySourceScanningFinished();
            return uniquePhotos;
        }

        public PhotoDao LoadData(Configuration config, Report report)
        {
            consoleView.DisplayDaoLoadingBeginning();

            PhotoDao photoDao = new PhotoDao(config.DataFile.FullName);

            LoadDaoInfoToReport(report, photoDao);

            consoleView.DisplayDaoLoadingFinished();
            return photoDao;
        }


        private Report UpdateReportWithNewImageCount(Configuration config, Report report, PhotoDao photoDao)
        {
            if (config.Landscape != null)
            {
                config.Landscape.Refresh();
                if (config.Landscape.Exists)
                    report.ImagesInLandscapeFolder = config.Landscape
                                                .GetFiles()
                                                .Count();
            }
            report.PostImageCount = photoDao.size();
            return report;
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

        private void moveUniquePhotosToAppropiateDirectory(IEnumerable<Photo> uniquePhotos,
                                        Configuration config,
                                        Report report)
        {
            foreach (Photo photo in uniquePhotos)
            {
                System.Drawing.Size dimension = photo.Dimension;
                try
                {
                    consoleView.CurrentFileNumberTransfering++;
                    CopyFilesToAppropriateDirectory(config, report, photo, dimension);
                }
                catch (System.IO.IOException ex)
                {
                    consoleView.DisplaySkippingPhotoDuringTransfer(photo, ex);
                }
            }
        }

        private void CopyFilesToAppropriateDirectory(Configuration config, Report report, Photo photo, System.Drawing.Size dimension)
        {
            if (dimension.IsEmpty)
            {
                if (!config.ImagesOnly)
                {
                    if (Utilities.MultimediaUtilities.IsMultimedia(new Uri(photo.Path)))
                    {
                        CopyToMediaDirectory(config, report, photo);
                    }
                    else
                    {
                        CopyToDataDirectory(config, report, photo);
                    }
                }
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

        private void CopyToDataDirectory(Configuration config, Report report, Photo photo)
        {
            CopyToDirectory(config, report, photo, config.DataDirectory);
        }

        private void CopyToMediaDirectory(Configuration config, Report report, Photo photo)
        {
            CopyToDirectory(config, report, photo, config.MultimediaDirectory);
        }

        private void CopyOtherImages(Configuration config, Report report, Photo photo)
        {
            if (!config.LargeImagesOnly)
            {
                CopyToDirectory(config, report, photo, config.SmallDirectory);
            }
        }

        private void CopyLargeImagesToApproprateDirectory(Configuration config, Report report, Photo photo, System.Drawing.Size dimension)
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

        private void CopyToDirectory(Configuration config, Report report, Photo photo, DirectoryInfo destinationDirectory)
        {
            string destinationFullName = GenerateNewFullName(config, photo, destinationDirectory);

            if (!config.Test)
            {
                CreateNonExistantDirectory(destinationDirectory);
                consoleView.DisplayCurrentFileTransfer(destinationFullName);
                if (config.Move)
                    photo.FileInfo.MoveTo(destinationFullName);
                else
                    photo.FileInfo.CopyTo(destinationFullName, false);

                consoleView.DisplayCompleteFileTransfer(destinationFullName);
            }
            else
            {
                consoleView.DisplayFileTestTransfer(photo.FileInfo.FullName, destinationFullName);
            }
            report.Moved++;
        }

        private string GenerateNewFullName(Configuration config, Photo photo, DirectoryInfo destinationDirectory)
        {
            if (config?.RebuildExtensions ?? false)
                return destinationDirectory.FullName + @"\" + RebuildImageExtension(photo, config.AggressiveExtensions);
            else
                return destinationDirectory.FullName + @"\" + photo.FileInfo.Name;
        }

        public string RebuildImageExtension(Photo photo, bool aggressive)
        {
            string properExtension = Utilities.ImageUtilities.DetectProperExtension(photo);

            if (string.IsNullOrWhiteSpace(properExtension) && Utilities.MultimediaUtilities.IsMultimedia(new Uri(photo.Path)))
            {
                properExtension = Utilities.MultimediaUtilities.DetectProperExtension(photo.Path);
            }

            if (string.IsNullOrWhiteSpace(properExtension))
            {
                properExtension = Utilities.MultimediaUtilities.DetectProperExtension(photo.Path);
            }

            if (string.IsNullOrWhiteSpace(properExtension))
            {
                return photo.FileInfo.Name;
            }
            else
            {
                return RemoveExtensions(photo, aggressive) + "." + properExtension;
            }

        }

        private string RemoveExtensions(Photo photo, bool aggresive)
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

        private void CreateNonExistantDirectory(DirectoryInfo directory)
        {
            if (!directory.Exists)
            {
                directory.Create();
                directory.Refresh();
            }
        }

        private IEnumerable<Photo> scanDirectoryForNewPhotos(DirectoryInfo sourceDirectory, PhotoDao photoDao, Report report, bool recurse)
        {
            FileInfo[] possiblePhotos = (recurse) ?
                                sourceDirectory.GetFiles("*", SearchOption.AllDirectories) :
                                        sourceDirectory.GetFiles("*", SearchOption.TopDirectoryOnly);

            int currentPosition = 0;
            int totalPossibles = possiblePhotos.Count();

            IEnumerable<Photo> photos = possiblePhotos.Select(possiblePhoto =>
            {
                consoleView.DisplayScanProgress(currentPosition++, totalPossibles);
                return PhotoBuilder.Build(possiblePhoto.FullName);
            })
                                                                                 .ToList();
            totalPossibles = possiblePhotos.Count();

            report.Scanned = photos.Count();

            IEnumerable<Photo> filteredPhotos = filterOutTheFilesWeAlreadyHave(photoDao, photos);

            report.NewPhotos = filteredPhotos.Count();
            report.AlreadyHad = report.Scanned - filteredPhotos.Count();

            filteredPhotos = GetDistinctPhotos(filteredPhotos);

            report.Distinct = filteredPhotos.Count();

            return filteredPhotos;
        }

        private IEnumerable<Photo> filterOutTheFilesWeAlreadyHave(PhotoDao photoDao, IEnumerable<Photo> photos)
        {

            List<Photo> photosWeDoNotHaveYet = new List<Photo>();
            int accepted = 0, rejected = 0, total = photos.Count();

            foreach (var photo in photos)
            {
                bool alreadyHaveThisOne = photoDao.Contains(photo);

                if (!alreadyHaveThisOne)
                {
                    photosWeDoNotHaveYet.Add(photo);
                    accepted = photosWeDoNotHaveYet.Count();
                    consoleView.DisplayAFileHasBeenAccepted(accepted, rejected, total);
                }
                else
                {
                    rejected++;
                    consoleView.DisplayAFileHasBeenFilteredOut(accepted, rejected, total);
                }
            }

            return photosWeDoNotHaveYet;
        }


        public IEnumerable<Photo> GetDistinctPhotos(IEnumerable<Photo> filteredPhotos)
        {
            return getDistinctPhotos(filteredPhotos);
        }

        private IEnumerable<Photo> getDistinctPhotos(IEnumerable<Photo> photos)
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
