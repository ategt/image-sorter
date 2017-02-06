using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using BackgroundImageSorter.Model;
using BackgroundImageSorter.Controller;

namespace BackgroundImageSorter.Tests
{
    [TestFixture]
    public class ProgramTests
    {
        static string workingDir = @"C:\Users\" + Environment.UserName + @"\Documents\Visual Studio 2015\Projects\BackgroundImageSorter\test_images";
        View.ConsoleView consoleView = null;
        Controller.IOController ioController = null;

        [SetUp]
        public void SetUp()
        {
            consoleView = new View.ConsoleView();
            ioController = new IOController(consoleView);
            DirectoryInfo outputDir = Directory.CreateDirectory(workingDir + @"\output");

            if ( outputDir.GetFiles().Count() > 0 || outputDir.GetDirectories().Count() > 0)
            {
                ResetOutputDirectory();
                //outputDir.Delete(true);
                //outputDir.Create();
            }
        }

        [TearDown]
        public void TearDown()
        {
            ResetOutputDirectory();
        }

        [Test()]
        public void GetDistinctPhotosTest()
        {
            DirectoryInfo sourceDirectory = new DirectoryInfo(workingDir + @"\input");
            FileInfo[] possiblePhotos = sourceDirectory.GetFiles();

            IEnumerable<Photo> photos = possiblePhotos.Select(possiblePhoto => PhotoBuilder.Build(possiblePhoto.FullName));

            var distinctPhotos = ApplicationController.GetDistinctPhotos(photos);

            Assert.IsTrue(distinctPhotos.Count() < 6);
            Assert.AreEqual(distinctPhotos.Count(), 3);

        }

        [Test()]
        public void ConfigInterpreterHelpTest()
        {

            string[] args = { "-h" };

            Configuration config = ConfigurationController.SetupConfiguration(args, ConfigurationBuilder.BuildConfig());

            Assert.NotNull(config);
            Assert.IsTrue(config.ShowHelp);

        }

        [Test()]
        [Timeout(100)]
        public void ConfigInterpreterActionTest()
        {
            string source = workingDir + @"\input";
            string destination = workingDir + @"\output";
            string data = workingDir + @"\output\photo.bin";

            string[] args = { "-h",
                              "-s", source,
                              "-o", destination,
                              "-d", data
                       };

            Report report = new ApplicationController(ioController, consoleView).RunProgram(args);

            DirectoryAssert.Exists(source);
            DirectoryAssert.Exists(destination);
            FileAssert.DoesNotExist(data);
            Assert.AreEqual(Directory.GetFiles(source).Count(), 6);
            Assert.AreEqual(Directory.GetFiles(destination).Count(), 0);

            Assert.Null(report);

        }

        [Test()]
        public void ConfigInterpreterTest()
        {
            string[] args = new string[] { "-d", workingDir + @"\data.bin",
                                    @"--s=" + workingDir + @"\input",
                                    @"/Output:" + workingDir + @"\output",
                                    "--Automated",
                                    "-t-" };

            Configuration config = ConfigurationController.SetupConfiguration(args, ConfigurationBuilder.BuildConfig());

            Assert.NotNull(config);
            Assert.NotNull(config.BackgroundDirectory);
            Assert.IsFalse(config.ShowHelp);

            Assert.IsTrue(config.SuppressReport);
            Assert.IsFalse(config.Test);
            Assert.IsFalse(config.Error);

            Assert.AreEqual(config.Portrait.FullName, workingDir + @"\output\Backgrounds\Portrait");
            Assert.AreEqual(config.Landscape.FullName, workingDir + @"\output\Backgrounds\Landscape");

        }

        [Test()]
        public void ConfigInterpreterWithCustomDestinationsTest()
        {
            string[] args = new string[] { "-d", workingDir + @"\data.bin",
                                    @"--s=" + workingDir + @"\input",
                                    @"/Output:" + workingDir + @"\output",
                                    "-p", workingDir + @"\portrait",
                                    "-l", workingDir + @"\landscape",
                                    "--Automated-",
                                    "-t+" };

            Configuration config = ConfigurationController.SetupConfiguration(args, ConfigurationBuilder.BuildConfig());

            Assert.NotNull(config);
            Assert.IsFalse(config.ShowHelp);

            Assert.IsFalse(config.SuppressReport);
            Assert.IsTrue(config.Test);
            Assert.IsFalse(config.Error);

            Assert.AreEqual(config.Portrait.FullName, workingDir + @"\portrait");
            Assert.AreEqual(config.Landscape.FullName, workingDir + @"\landscape");
        }

        [Test()]
        public void MainProgramTest()
        {
            //ResetOutputDirectory();

            DirectoryInfo portraitDir = new DirectoryInfo(workingDir + @"\output\portrait");
            DirectoryInfo landscapeDir = new DirectoryInfo(workingDir + @"\output\landscape");
            DirectoryInfo sourceDir = new DirectoryInfo(workingDir + @"\input");


            string[] args = new string[] { "-d", workingDir + @"\output\data.bin",
                                    $"--s={sourceDir.FullName}",
                                    @"/Output:" + workingDir + @"\output",
                                    "-p", portraitDir.FullName,
                                    "-l", landscapeDir.FullName};

            Report report = new ApplicationController(ioController, consoleView).RunProgram(args);

            FileInfo[] landscapePhotos = landscapeDir.GetFiles();

            Assert.AreEqual(landscapePhotos.Count(), 3);

            DirectoryAssert.DoesNotExist(portraitDir);

            Assert.AreEqual(report.Scanned, 6);
            Assert.AreEqual(report.AlreadyHad, 0);
            Assert.AreEqual(report.Distinct, 3);
            Assert.AreEqual(report.Moved, 3);
            Assert.AreEqual(report.ImagesInLandscapeFolder, 3);

        }

        [Test()]
        public void MainProgramSingleOutputTest()
        {

            DirectoryInfo sourceDir = new DirectoryInfo(workingDir + @"\input");
            DirectoryInfo destinationDir = new DirectoryInfo(workingDir + @"\output");
            DirectoryInfo portraitDir = new DirectoryInfo(workingDir + @"\output\portrait");
            DirectoryInfo landscapeDir = new DirectoryInfo(workingDir + @"\output\landscape");

            string[] args = new string[] { "-d", workingDir + @"\output\data.bin",
                                    $"--s={sourceDir.FullName}",
                                    @"/Output:" + workingDir + @"\output",
                                    "-Single",
                                    "-sub" };

            Report report = new ApplicationController(ioController, consoleView).RunProgram(args);

            FileInfo[] outputPhotos = destinationDir.GetFiles();

            Assert.AreEqual(outputPhotos.Count(), 7);

            Assert.AreEqual(destinationDir.GetDirectories().Count(), 0);
            
            DirectoryAssert.DoesNotExist(portraitDir);
            DirectoryAssert.DoesNotExist(landscapeDir);

            Assert.AreEqual(report.Scanned, 9);
            Assert.AreEqual(report.AlreadyHad, 0);
            Assert.AreEqual(report.Distinct, 6);
            Assert.AreEqual(report.Moved, 6);
            //Assert.AreEqual(report.ImagesInLandscapeFolder, 6);

            //ResetOutputDirectory();

        }

        [Test()]
        public void SetDirectoriesManuallyTest()
        {
            //ResetOutputDirectory();

            Configuration config = new Configuration
            {
                Source = new DirectoryInfo(workingDir + @"\input"),
                Destination = new DirectoryInfo(workingDir + @"\output"),
                Portrait = new DirectoryInfo(workingDir + @"\output\portrait"),
                Landscape = new DirectoryInfo(workingDir + @"\output\landscape"),
                DataFile = new FileInfo(workingDir + @"\data.bin"),
                NoUpdate = true
            };

            config = ConfigurationController.SetDefaultDirectories(config);

            string testImageDirectory = workingDir + @"\";

            Assert.AreEqual(testImageDirectory + @"output\Other Images", config.SmallDirectory.FullName);
            Assert.AreEqual(testImageDirectory + @"output\Data", config.DataDirectory.FullName);
            Assert.AreEqual(testImageDirectory + @"output\Backgrounds", config.BackgroundDirectory.FullName);
            Assert.AreEqual(testImageDirectory + @"output\landscape", config.Landscape.FullName);
            Assert.AreEqual(testImageDirectory + @"output\portrait", config.Portrait.FullName);

            Assert.IsFalse(config.SmallDirectory.Exists);
            Assert.IsFalse(config.DataDirectory.Exists);
            Assert.IsFalse(config.BackgroundDirectory.Exists);
            Assert.IsFalse(config.Landscape.Exists);
            Assert.IsFalse(config.Portrait.Exists);

        }

        [Test()]
        public void SetDefaultDirectoriesTest()
        {
            Configuration config = new Configuration
            {
                Source = new DirectoryInfo(workingDir + @"\input"),
                Destination = new DirectoryInfo(workingDir + @"\output"),
                DataFile = new FileInfo(workingDir + @"\data.bin"),
                NoUpdate = true
            };

            config = ConfigurationController.SetDefaultDirectories(config);

            Assert.AreEqual(workingDir + @"\output\Other Images", config.SmallDirectory.FullName);
            Assert.AreEqual(workingDir + @"\output\Data", config.DataDirectory.FullName);
            Assert.AreEqual(workingDir + @"\output\Backgrounds", config.BackgroundDirectory.FullName);
            Assert.AreEqual(workingDir + @"\output\Backgrounds\Landscape", config.Landscape.FullName);
            Assert.AreEqual(workingDir + @"\output\Backgrounds\Portrait", config.Portrait.FullName);
        }

        [Test()]
        public void MainActionTest()
        {
            //ResetOutputDirectory();
            //Directory.CreateDirectory(workingDir + @"\output");

            Configuration config = new Configuration
            {
                Source = new DirectoryInfo(workingDir + @"\input"),
                Destination = new DirectoryInfo(workingDir + @"\output"),
                Landscape = new DirectoryInfo(workingDir + @"\output\landscape"),
                DataFile = new FileInfo(workingDir + @"\data.bin"),
                NoUpdate = true
            };

            Assert.AreEqual(6, config.Source.GetFiles().Count());

            Report report = new ApplicationController(ioController, consoleView).SortImages(config, new Report());

            DirectoryInfo landscapes = config.Landscape;
            FileInfo[] landscapePhotos = landscapes.GetFiles();

            Assert.AreEqual(landscapePhotos.Count(), 3);

            Assert.IsNull(config.Portrait);

            Assert.AreEqual(report.Scanned, 6);
            Assert.AreEqual(report.AlreadyHad, 0);
            Assert.AreEqual(report.Distinct, 3);
            Assert.AreEqual(report.Moved, 3);
            Assert.AreEqual(report.ImagesInLandscapeFolder, 3);

            Assert.AreEqual(6, config.Source.GetFiles().Count());

            //ResetOutputDirectory();

        }

        [Test()]
        public void RecursiveActionTest()
        {
            Configuration config = new Configuration
            {
                Source = new DirectoryInfo(workingDir + @"\input"),
                Destination = new DirectoryInfo(workingDir + @"\output"),
                DataFile = new FileInfo(workingDir + @"\data.bin"),
                Recurse = true,
                NoUpdate = true
            };

            config = ConfigurationController.SetDefaultDirectories(config);

            Assert.AreEqual(9, config.Source.GetFiles("*", SearchOption.AllDirectories).Count());

            Report report = new ApplicationController(ioController, consoleView).SortImages(config, new Report());

            DirectoryInfo landscapes = config.Landscape;
            FileInfo[] landscapePhotos = landscapes.GetFiles();

            Assert.AreEqual(landscapePhotos.Count(), 3);

            DirectoryAssert.Exists(config.Portrait);
            Assert.IsNotNull(config.Portrait);

            DirectoryAssert.Exists(config.Landscape);
            Assert.IsNotNull(config.Landscape);

            DirectoryAssert.Exists(config.SmallDirectory);
            Assert.IsNotNull(config.SmallDirectory);

            DirectoryAssert.Exists(config.DataDirectory);
            Assert.IsNotNull(config.DataDirectory);

            FileAssert.Exists(workingDir + @"\output\Backgrounds\Portrait\76a834734a7ce31bab6f778642c41b1ad4675f56b57c7881bf69e2e1be095d4d.jpg.jpg");
            FileAssert.Exists(workingDir + @"\output\Data\Random Text");
            FileAssert.Exists(workingDir + @"\output\Other Images\Small Image");

            Assert.AreEqual(report.Scanned, 9);
            Assert.AreEqual(report.AlreadyHad, 0);
            Assert.AreEqual(report.Distinct, 6);
            Assert.AreEqual(report.Moved, 6);
            Assert.AreEqual(report.ImagesInLandscapeFolder, 3);

            Assert.AreEqual(6, config.Source.GetFiles("*", SearchOption.TopDirectoryOnly).Count());
            Assert.AreEqual(9, config.Source.GetFiles("*", SearchOption.AllDirectories).Count());
            Assert.AreEqual(6, config.Destination.GetFiles("*", SearchOption.AllDirectories).Count());
            Assert.AreEqual(1, config.DataDirectory.GetFiles("*", SearchOption.AllDirectories).Count());
            Assert.AreEqual(1, config.SmallDirectory.GetFiles("*", SearchOption.AllDirectories).Count());
            Assert.AreEqual(1, config.Portrait.GetFiles("*", SearchOption.AllDirectories).Count());

            //ResetOutputDirectory();

        }

        [Test]
        public void RunThrice()
        {
            Configuration config = new Configuration
            {
                Source = new DirectoryInfo(workingDir + @"\input"),
                Destination = new DirectoryInfo(workingDir + @"\output"),
                Landscape = new DirectoryInfo(workingDir + @"\output\landscape"),
                DataFile = new FileInfo(workingDir + @"\output\data.bin"),                
            };

            Report report = new ApplicationController(ioController, consoleView).SortImages(config, new Report());

            Assert.AreEqual(report.StoredBackgrounds, 0);
            Assert.AreEqual(report.StoredFiles, 0);
            Assert.AreEqual(report.StoredImages, 0);

            Assert.AreEqual(report.Scanned, 6);
            Assert.AreEqual(report.NewPhotos, 6);
            Assert.AreEqual(report.Distinct, 3);
            Assert.AreEqual(report.AlreadyHad, 0);
            Assert.AreEqual(report.Moved, 3);

            Configuration secondConfig = new Configuration
            {
                Source = new DirectoryInfo(workingDir + @"\input"),
                Destination = new DirectoryInfo(workingDir + @"\output"),
                Landscape = new DirectoryInfo(workingDir + @"\output\landscape"),
                DataFile = new FileInfo(workingDir + @"\output\data.bin"),
            };

            Report secondReport = new ApplicationController(ioController, consoleView).SortImages(secondConfig, new Report());

            Assert.AreEqual(secondReport.StoredBackgrounds, 3);
            Assert.AreEqual(secondReport.StoredFiles, 4);
            Assert.AreEqual(secondReport.StoredImages, 3);

            Assert.AreEqual(secondReport.Scanned, 6);
            Assert.AreEqual(secondReport.NewPhotos, 0);
            Assert.AreEqual(secondReport.AlreadyHad, 6);
            Assert.AreEqual(secondReport.Distinct, 0);
            Assert.AreEqual(secondReport.Moved, 0);

            Configuration thirdConfig = new Configuration
            {
                Source = new DirectoryInfo(workingDir + @"\input"),
                Destination = new DirectoryInfo(workingDir + @"\output"),
                Landscape = new DirectoryInfo(workingDir + @"\output\landscape"),
                DataFile = new FileInfo(workingDir + @"\output\data2.bin"),
                Recurse = true,
                PreScan = true
            };

            thirdConfig = ConfigurationController.SetDefaultDirectories(thirdConfig);

            Report thirdReport = new ApplicationController(ioController, consoleView).SortImages(thirdConfig, new Report());

            Assert.AreEqual(thirdReport.StoredBackgrounds, 0);
            Assert.AreEqual(thirdReport.StoredFiles, 0);
            Assert.AreEqual(thirdReport.StoredImages, 0);

            Assert.AreEqual(thirdReport.Scanned, 9);
            Assert.AreEqual(thirdReport.NewPhotos, 3);
            Assert.AreEqual(thirdReport.AlreadyHad, 6);
            Assert.AreEqual(thirdReport.Distinct, 3);
            Assert.AreEqual(thirdReport.Moved, 3);

        }

        [Test]
        public void ImageExtensionRebuilderLazily()
        {
            Photo photo = PhotoBuilder.Build(workingDir + @"\input\4aaf59bdb816c76e7b2983a48ef907833ecae3f5cef6fb02a3fbfa36274e1cc6.jpg.jpg");

            string improvedName = ApplicationController.RebuildImageExtension(photo, false);

            Assert.AreEqual(improvedName, "4aaf59bdb816c76e7b2983a48ef907833ecae3f5cef6fb02a3fbfa36274e1cc6.jpg.jpg");

        }

        [Test]
        public void ImageExtensionRebuilderAggresively()
        {
            Photo photo = PhotoBuilder.Build(workingDir + @"\input\4aaf59bdb816c76e7b2983a48ef907833ecae3f5cef6fb02a3fbfa36274e1cc6.jpg.jpg");

            string improvedName = ApplicationController.RebuildImageExtension(photo, true);

            Assert.AreEqual(improvedName, "4aaf59bdb816c76e7b2983a48ef907833ecae3f5cef6fb02a3fbfa36274e1cc6.jpg");

        }

        [Test]
        public void ImageExtensionRebuilderB()
        {
            Photo photo = PhotoBuilder.Build(workingDir + @"\input\07d13665d2a42b6a9b1308d76ea9d43ede14964ac843f5c646e11d8537323c75.png");

            string improvedName = ApplicationController.RebuildImageExtension(photo, true);

            Assert.AreEqual(improvedName, "07d13665d2a42b6a9b1308d76ea9d43ede14964ac843f5c646e11d8537323c75.jpg");
        }

        [Test]
        public void ImageExtensionRebuilderC()
        {
            Photo photo = PhotoBuilder.Build(workingDir + @"\input\07d13665d2a42b6a9b1308d76ea9d43ede14964ac843f5c646e11d8537323c75.png");

            string improvedName = ApplicationController.RebuildImageExtension(photo, false);

            Assert.AreEqual(improvedName, "07d13665d2a42b6a9b1308d76ea9d43ede14964ac843f5c646e11d8537323c75.jpg");
        }


        private static void ResetOutputDirectory()
        {
            try
            {
                Directory.Delete(workingDir + @"\output", true);
            }
            catch (DirectoryNotFoundException ex)
            {

            }

            Assert.IsFalse(Directory.Exists(workingDir + @"\output"));
            DirectoryAssert.DoesNotExist(workingDir + @"\output");

            Directory.CreateDirectory(workingDir + @"\output");

            Assert.AreEqual(new DirectoryInfo(workingDir + @"\input").GetFiles().Count(),
                    6);
        }
    }
}