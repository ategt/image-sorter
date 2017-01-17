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
        public void ConfigInterpreterTest()
        {

            string[] args = { "-h" };

            Configuration config = Program.SetupConfiguration(args, Program.BuildConfig());

            Assert.NotNull(config);
            Assert.IsTrue(config.ShowHelp);

            args = new string[] { "-d", @"C:\Users\ATeg\Documents\Visual Studio 2015\Projects\BackgroundImageSorter\test_images\data.bin",
                                    @"--s=C:\Users\ATeg\Documents\Visual Studio 2015\Projects\BackgroundImageSorter\test_images\input",
                                    @"/Output:C:\Users\ATeg\Documents\Visual Studio 2015\Projects\BackgroundImageSorter\test_images\output",
                                    "--Automated",
                                    "-t-" };

            config = Program.SetupConfiguration(args, Program.BuildConfig());

            Assert.NotNull(config);
            Assert.NotNull(config.BackgroundDirectory);
            Assert.IsFalse(config.ShowHelp);

            Assert.IsTrue(config.SuppressReport);
            Assert.IsFalse(config.Test);
            Assert.IsFalse(config.Error);

            Assert.AreEqual(config.Portrait.FullName, @"C:\Users\ATeg\Documents\Visual Studio 2015\Projects\BackgroundImageSorter\test_images\output\Backgrounds\Portrait");
            Assert.AreEqual(config.Landscape.FullName, @"C:\Users\ATeg\Documents\Visual Studio 2015\Projects\BackgroundImageSorter\test_images\output\Backgrounds\Landscape");

            args = new string[] { "-d", @"C:\Users\ATeg\Documents\Visual Studio 2015\Projects\BackgroundImageSorter\test_images\data.bin",
                                    @"--s=C:\Users\ATeg\Documents\Visual Studio 2015\Projects\BackgroundImageSorter\test_images\input",
                                    @"/Output:C:\Users\ATeg\Documents\Visual Studio 2015\Projects\BackgroundImageSorter\test_images\output",
                                    "-p", @"C:\Users\ATeg\Documents\Visual Studio 2015\Projects\BackgroundImageSorter\test_images\portrait",
                                    "-l", @"C:\Users\ATeg\Documents\Visual Studio 2015\Projects\BackgroundImageSorter\test_images\landscape",
                                    "--Automated-",
                                    "-t+" };

            config = Program.SetupConfiguration(args, Program.BuildConfig());

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


        }

        [Test()]
        public void MainActionTest()
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

            Assert.AreEqual(6, config.Source.GetFiles().Count());

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

            Assert.AreEqual(6, config.Source.GetFiles().Count());

        }
    }
}