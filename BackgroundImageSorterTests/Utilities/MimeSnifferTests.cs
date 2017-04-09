using BackgroundImageSorter.Utilities;
using NUnit.Framework;

namespace BackgroundImageSorterTests.Utilities
{
    [TestFixture]
    public class MimeSnifferTests
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
        public void TestGetMime()
        {
            string mediaFolder = @"C:\Users\" + System.Environment.UserName + @"\Documents\Visual Studio 2015\Projects\BackgroundImageSorter\test_images\special\media";

            string audio = System.IO.Path.Combine(mediaFolder, "audio");
            string audioMime = MimeSniffer.GetMime(audio);
            Assert.AreEqual(audioMime, "application/octet-stream");

            string video = System.IO.Path.Combine(mediaFolder, "video");
            string videoMime = MimeSniffer.GetMime(video);
            Assert.AreEqual(videoMime, "application/octet-stream");

            string audioAndVideo = System.IO.Path.Combine(mediaFolder, "audio and video");
            string audioAndVideoMime = MimeSniffer.GetMime(audioAndVideo);
            Assert.AreEqual(audioAndVideoMime, "application/octet-stream");

            string inputPath = System.IO.Path.Combine(mediaFolder, "..\\trouble files\\files.txt");
            string outputMime = MimeSniffer.GetMime(inputPath);
            Assert.AreEqual(outputMime, "text/plain");

            inputPath = System.IO.Path.Combine(mediaFolder, "..\\trouble files\\WinZip Archive (1378)");
            outputMime = MimeSniffer.GetMime(inputPath);
            Assert.AreEqual(outputMime, "application/x-zip-compressed");

            inputPath = System.IO.Path.Combine(mediaFolder, "..\\trouble files\\Zip Archive (269250)");
            outputMime = MimeSniffer.GetMime(inputPath);
            Assert.AreEqual(outputMime, "application/x-zip-compressed");

            inputPath = System.IO.Path.Combine(mediaFolder, "..\\trouble files\\download.png");
            outputMime = MimeSniffer.GetMime(inputPath);
            Assert.AreEqual(outputMime, "image/png");

            inputPath = System.IO.Path.Combine(mediaFolder, "..\\trouble files\\16464157_924752977628291_4243522177728512000_n.jpg");
            outputMime = MimeSniffer.GetMime(inputPath);
            Assert.AreEqual(outputMime, "image/jpeg");

        }

    }
}