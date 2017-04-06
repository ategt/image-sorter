using BackgroundImageSorter.Utilities;
using NUnit.Framework;
using System.IO;

namespace BackgroundImageSorterTests.Utilities
{
    [TestFixture]
    public class MultimediaUtilitiesTests
    {
        [SetUp]
        public void TestInitialize()
        {
        }

        [TearDown]
        public void TestCleanup()
        {
        }

        [Test]
        public void SimulateProblemFiles()
        {
            string workingDir = @"C:\Users\" + System.Environment.UserName + @"\Documents\Visual Studio 2015\Projects\BackgroundImageSorter\test_images\special\trouble files";

            string[] problemFiles = Directory.GetFiles(workingDir);

            foreach (string file in problemFiles)
            {
                FileInfo fileInfo = new FileInfo(file);
                if (MultimediaUtilities.IsMultimedia(new System.Uri(fileInfo.FullName)))
                {
                    string extenstion = MultimediaUtilities.DetectProperExtension(fileInfo.FullName);
                    Assert.NotNull(extenstion);
                }
                else
                {
                    string extenstion = MultimediaUtilities.DetectProperExtension(fileInfo.FullName);
                    Assert.Null(extenstion);
                }
            }
        }
    }
}