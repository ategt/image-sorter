using NUnit.Framework;
using BackgroundImageSorter;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using BackgroundImageSorter.Model;

namespace BackgroundImageSorter.Tests
{
    [TestFixture()]
    public class ProgramTests
    {
        [Test()]
        public void GetDistinctPhotosTest()
        {
            DirectoryInfo sourceDirectory = new DirectoryInfo(@"C:\Users\ATeg\Documents\Visual Studio 2015\Projects\BackgroundImageSorter\test_images\input");
            FileInfo[] possiblePhotos = sourceDirectory.GetFiles();

            IEnumerable<Photo> photos = possiblePhotos.Select(possiblePhoto => PhotoBuilder.Build(possiblePhoto.FullName));

            var distinctPhotos = Program.GetDistinctPhotos(photos);

            Assert.IsTrue(distinctPhotos.Count() < 6);
            Assert.AreEqual(distinctPhotos.Count(), 3);

        }

        [Test()]
        public void ConfigInterpreterHelpTest()
        {

            string[] args = { "-h" };

            Configuration config = Program.SetupConfiguration(args, Program.BuildConfig());

            Assert.NotNull(config);
            Assert.IsTrue(config.ShowHelp);
        }

        [Test()]
        public void ConfigInterpreterTest()
        {
            string[] args = new string[] { "-d", @"C:\Users\ATeg\Documents\Visual Studio 2015\Projects\BackgroundImageSorter\test_images\data.bin",
                                    @"--s=C:\Users\ATeg\Documents\Visual Studio 2015\Projects\BackgroundImageSorter\test_images\input",
                                    @"/Output:C:\Users\ATeg\Documents\Visual Studio 2015\Projects\BackgroundImageSorter\test_images\output",
                                    "--Automated",
                                    "-t-" };

            Configuration config = Program.SetupConfiguration(args, Program.BuildConfig());

            Assert.NotNull(config);
            Assert.NotNull(config.BackgroundDirectory);
            Assert.IsFalse(config.ShowHelp);

            Assert.IsTrue(config.SuppressReport);
            Assert.IsFalse(config.Test);
            Assert.IsFalse(config.Error);

            Assert.AreEqual(config.Portrait.FullName, @"C:\Users\ATeg\Documents\Visual Studio 2015\Projects\BackgroundImageSorter\test_images\output\Backgrounds\Portrait");
            Assert.AreEqual(config.Landscape.FullName, @"C:\Users\ATeg\Documents\Visual Studio 2015\Projects\BackgroundImageSorter\test_images\output\Backgrounds\Landscape");

        }

        [Test()]
        public void ConfigInterpreterWithCustomDestinationsTest()
        {
            string[] args = new string[] { "-d", @"C:\Users\ATeg\Documents\Visual Studio 2015\Projects\BackgroundImageSorter\test_images\data.bin",
                                    @"--s=C:\Users\ATeg\Documents\Visual Studio 2015\Projects\BackgroundImageSorter\test_images\input",
                                    @"/Output:C:\Users\ATeg\Documents\Visual Studio 2015\Projects\BackgroundImageSorter\test_images\output",
                                    "-p", @"C:\Users\ATeg\Documents\Visual Studio 2015\Projects\BackgroundImageSorter\test_images\portrait",
                                    "-l", @"C:\Users\ATeg\Documents\Visual Studio 2015\Projects\BackgroundImageSorter\test_images\landscape",
                                    "--Automated-",
                                    "-t+" };

            Configuration config = Program.SetupConfiguration(args, Program.BuildConfig());

            Assert.NotNull(config);
            Assert.IsFalse(config.ShowHelp);

            Assert.IsFalse(config.SuppressReport);
            Assert.IsTrue(config.Test);
            Assert.IsFalse(config.Error);

            Assert.AreEqual(config.Portrait.FullName, @"C:\Users\ATeg\Documents\Visual Studio 2015\Projects\BackgroundImageSorter\test_images\portrait");
            Assert.AreEqual(config.Landscape.FullName, @"C:\Users\ATeg\Documents\Visual Studio 2015\Projects\BackgroundImageSorter\test_images\landscape");
        }

        [Test()]
        public void MainProgramTest()
        {
            Directory.Delete(@"C:\Users\ATeg\Documents\Visual Studio 2015\Projects\BackgroundImageSorter\test_images\output", true);
            Directory.CreateDirectory(@"C:\Users\ATeg\Documents\Visual Studio 2015\Projects\BackgroundImageSorter\test_images\output");

            Configuration config = new Configuration
            {
                Source = new DirectoryInfo(@"C:\Users\ATeg\Documents\Visual Studio 2015\Projects\BackgroundImageSorter\test_images\input"),
                Destination = new DirectoryInfo(@"C:\Users\ATeg\Documents\Visual Studio 2015\Projects\BackgroundImageSorter\test_images\output"),
                DataFile = new FileInfo(@"C:\Users\ATeg\Documents\Visual Studio 2015\Projects\BackgroundImageSorter\test_images\data.bin"),
                NoUpdate = true
            };

            config = Program.SetDefaultDirectories(config);

            Report report = Program.CommitPurpose(config, new Report());

            DirectoryInfo landscapes = config.Landscape;
            FileInfo[] landscapePhotos = landscapes.GetFiles();

            Assert.AreEqual(@"C:\Users\ATeg\Documents\Visual Studio 2015\Projects\BackgroundImageSorter\test_images\output\Backgrounds\Landscape", config.Landscape.FullName);

            Assert.AreEqual(landscapePhotos.Count(), 3);

            Assert.IsNull(config.Portrait);

            Assert.AreEqual(report.Scanned, 6);
            Assert.AreEqual(report.Skipped, 3);
            Assert.AreEqual(report.Moved, 3);
            Assert.AreEqual(report.ImagesInLandscapeFolder, 3);

            ResetOutputDirectory();

        }

        [Test()]
        public void SetDirectoriesManuallyTest()
        {
            ResetOutputDirectory();

            Configuration config = new Configuration
            {
                Source = new DirectoryInfo(@"C:\Users\ATeg\Documents\Visual Studio 2015\Projects\BackgroundImageSorter\test_images\input"),
                Destination = new DirectoryInfo(@"C:\Users\ATeg\Documents\Visual Studio 2015\Projects\BackgroundImageSorter\test_images\output"),
                Portrait = new DirectoryInfo(@"C:\Users\ATeg\Documents\Visual Studio 2015\Projects\BackgroundImageSorter\test_images\output\portrait"),
                Landscape = new DirectoryInfo(@"C:\Users\ATeg\Documents\Visual Studio 2015\Projects\BackgroundImageSorter\test_images\output\landscape"),
                DataFile = new FileInfo(@"C:\Users\ATeg\Documents\Visual Studio 2015\Projects\BackgroundImageSorter\test_images\data.bin"),
                NoUpdate = true
            };

            config = Program.SetDefaultDirectories(config);

            string testImageDirectory = @"C:\Users\ATeg\Documents\Visual Studio 2015\Projects\BackgroundImageSorter\test_images\";

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
                Source = new DirectoryInfo(@"C:\Users\ATeg\Documents\Visual Studio 2015\Projects\BackgroundImageSorter\test_images\input"),
                Destination = new DirectoryInfo(@"C:\Users\ATeg\Documents\Visual Studio 2015\Projects\BackgroundImageSorter\test_images\output"),
                DataFile = new FileInfo(@"C:\Users\ATeg\Documents\Visual Studio 2015\Projects\BackgroundImageSorter\test_images\data.bin"),
                NoUpdate = true
            };

            config = Program.SetDefaultDirectories(config);

            Assert.AreEqual(@"C:\Users\ATeg\Documents\Visual Studio 2015\Projects\BackgroundImageSorter\test_images\output\Other Images", config.SmallDirectory.FullName);
            Assert.AreEqual(@"C:\Users\ATeg\Documents\Visual Studio 2015\Projects\BackgroundImageSorter\test_images\output\Data", config.DataDirectory.FullName);
            Assert.AreEqual(@"C:\Users\ATeg\Documents\Visual Studio 2015\Projects\BackgroundImageSorter\test_images\output\Backgrounds", config.BackgroundDirectory.FullName);
            Assert.AreEqual(@"C:\Users\ATeg\Documents\Visual Studio 2015\Projects\BackgroundImageSorter\test_images\output\Backgrounds\Landscape", config.Landscape.FullName);
            Assert.AreEqual(@"C:\Users\ATeg\Documents\Visual Studio 2015\Projects\BackgroundImageSorter\test_images\output\Backgrounds\Portrait", config.Portrait.FullName);
        }

        [Test()]
        public void MainActionTest()
        {
            ResetOutputDirectory();
            Directory.CreateDirectory(@"C:\Users\ATeg\Documents\Visual Studio 2015\Projects\BackgroundImageSorter\test_images\output");

            Configuration config = new Configuration
            {
                Source = new DirectoryInfo(@"C:\Users\ATeg\Documents\Visual Studio 2015\Projects\BackgroundImageSorter\test_images\input"),
                Destination = new DirectoryInfo(@"C:\Users\ATeg\Documents\Visual Studio 2015\Projects\BackgroundImageSorter\test_images\output"),
                //Portrait = new DirectoryInfo(@"C:\Users\ATeg\Documents\Visual Studio 2015\Projects\BackgroundImageSorter\test_images\output\portrait"),
                Landscape = new DirectoryInfo(@"C:\Users\ATeg\Documents\Visual Studio 2015\Projects\BackgroundImageSorter\test_images\output\landscape"),
                DataFile = new FileInfo(@"C:\Users\ATeg\Documents\Visual Studio 2015\Projects\BackgroundImageSorter\test_images\data.bin"),
                NoUpdate = true
            };

            //config = Program.SetDefaultDirectories(config);

            Assert.AreEqual(6, config.Source.GetFiles().Count());

            Report report = Program.CommitPurpose(config, new Report());

            DirectoryInfo landscapes = config.Landscape;
            FileInfo[] landscapePhotos = landscapes.GetFiles();

            //Assert.AreEqual(@"C:\Users\ATeg\Documents\Visual Studio 2015\Projects\BackgroundImageSorter\test_images\output\Backgrounds\Landscape", config.Landscape.FullName);

            Assert.AreEqual(landscapePhotos.Count(), 3);

            Assert.IsNull(config.Portrait);

            Assert.AreEqual(report.Scanned, 6);
            Assert.AreEqual(report.Skipped, 3);
            Assert.AreEqual(report.Moved, 3);
            Assert.AreEqual(report.ImagesInLandscapeFolder, 3);

            Assert.AreEqual(6, config.Source.GetFiles().Count());

            ResetOutputDirectory();

        }

        private static void ResetOutputDirectory()
        {
            try
            {
                Directory.Delete(@"C:\Users\ATeg\Documents\Visual Studio 2015\Projects\BackgroundImageSorter\test_images\output", true);
            } catch (DirectoryNotFoundException ex)
            {

            }
            Assert.IsFalse(Directory.Exists(@"C:\Users\ATeg\Documents\Visual Studio 2015\Projects\BackgroundImageSorter\test_images\output"));
            DirectoryAssert.DoesNotExist(@"C:\Users\ATeg\Documents\Visual Studio 2015\Projects\BackgroundImageSorter\test_images\output");

            Directory.CreateDirectory(@"C:\Users\ATeg\Documents\Visual Studio 2015\Projects\BackgroundImageSorter\test_images\output");
        }
    }
}