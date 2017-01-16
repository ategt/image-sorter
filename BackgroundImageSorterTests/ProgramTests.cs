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
            DirectoryInfo sourceDirectory = new DirectoryInfo(@"C:\Users\ATeg\Desktop\tests\input");
            FileInfo[] possiblePhotos = sourceDirectory.GetFiles();

            IEnumerable<Photo> photos = possiblePhotos.Select(possiblePhoto => PhotoBuilder.Build(possiblePhoto.FullName));

            var distinctPhotos = Program.GetDistinctPhotos(photos);
            int counrt = distinctPhotos.Count();

            Assert.IsTrue(distinctPhotos.Count() < 6);

            Assert.AreEqual(distinctPhotos.Count(), 3);

        }
    }
}