﻿using BackgroundImageSorter.Model;
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
        ConsoleView consoleView = null;
        PhotoDao photoDao = null;

        public ApplicationController(IOController ioController, ConsoleView consoleView, PhotoDao photoDao)
        {
            this.ioController = ioController;
            this.consoleView = consoleView;
            this.photoDao = photoDao;
        }

        public static void Program(string[] args, IOController ioController, ConsoleView consoleView, PhotoDao photoDao)
        {
            Report report = new BackgroundImageSorter
                                                .Controller
                                                .ApplicationController(ioController, consoleView, photoDao)
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
            PhotoDao photoDao;
            IEnumerable<Photo> uniquePhotos =
            FindUniquePhotos(config, report);

            CopyOrMoveFiles(config, report, uniquePhotos);

            if (report.Moved > 0)
                UpdateData(config, report);
            else
                consoleView.DisplayNoFilesMoved();

            return ProcessReport(config, report);
        }

        private IEnumerable<Photo> FindUniquePhotos(Configuration config, Report report)
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
            ConsoleView.DisplayBeginingDirectoryPrescan();

            ioController.updateDirectoryData(config, photoDao);

            ConsoleView.DisplayFinishedDirectoryPrescan();
        }

        private static Report ProcessReport(Configuration config, Report report)
        {
            if (config.SuppressReport) return null;

            return report;
        }

        private void UpdateData(Configuration config, Report report)
        {
            ConsoleView.DisplayBeginUpdatingDao();

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

            ConsoleView.DisplayFinishedUpdateingDao();
        }


        private static void CopyOrMoveFiles(Configuration config, Report report, IEnumerable<Photo> uniquePhotos)
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


        private static Report UpdateReportWithNewImageCount(Configuration config, Report report, PhotoDao photoDao)
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
            if (dimension.IsEmpty)
            {
                if (!config.ImagesOnly)
                {
                    CopyToDataDirectory(config, report, photo);
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

        private static void CopyToDataDirectory(Configuration config, Report report, Photo photo)
        {
            CopyToDirectory(config, report, photo, config.DataDirectory);
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
            string destinationFullName = GenerateNewFullName(config, photo, destinationDirectory);

            if (!config.Test)
            {
                CreateNonExistantDirectory(destinationDirectory);
                if (config.Move)
                    photo.FileInfo.MoveTo(destinationFullName);
                else
                    photo.FileInfo.CopyTo(destinationFullName, false);
            }
            else
            {
                ConsoleView.DisplayFileTestTransfer(photo.FileInfo.FullName, destinationFullName);
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
            string properExtension = Utilities.ImageUtilities.DetectProperExtension(photo);

            if (string.IsNullOrWhiteSpace(properExtension))
            {
                return photo.FileInfo.Name;
            }
            else
            {
                return RemoveExtensions(photo, aggressive) + "." + properExtension;
            }

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

            int currentPosition = 0;
            int totalPossibles = possiblePhotos.Count();

            IEnumerable<Photo> photos = possiblePhotos.Select(possiblePhoto =>
            {
                ConsoleView.DisplayScanProgress(currentPosition++, totalPossibles);
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

        private static IEnumerable<Photo> filterOutTheFilesWeAlreadyHave(PhotoDao photoDao, IEnumerable<Photo> photos)
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
                    ConsoleView.DisplayAFileHasBeenAccepted(accepted, rejected, total);
                }
                else
                {
                    rejected++;
                    ConsoleView.DisplayAFileHasBeenFilteredOut(accepted, rejected, total);
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
