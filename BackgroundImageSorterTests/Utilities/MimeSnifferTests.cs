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
        public void TestMethod1()
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

        }

    }
}