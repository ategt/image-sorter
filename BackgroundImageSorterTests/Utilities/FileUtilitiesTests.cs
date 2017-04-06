using BackgroundImageSorter.Utilities;
using NUnit.Framework;

namespace BackgroundImageSorterTests.Utilities
{
    [TestFixture]
    public class FileUtilitiesTests
    {
        [SetUp]
        public void TestInitialize()
        {
        }

        [TearDown]
        public void TestCleanup()
        {
        }

        //[Test]
        public void TestMethod1()
        {
            //FileUtilities fileUtilities = FileUtilities.DetectProperExtension();
            //this.CreateFileUtilities();

            string workingDir = @"C:\Users\" + System.Environment.UserName + @"\Documents\Visual Studio 2015\Projects\BackgroundImageSorter\test_images\special\trouble files";

            string[] problemFiles = System.IO.Directory.GetFiles(workingDir);

            foreach (string file in problemFiles)
            {
                //string detectedExtension = FileUtilities.DetectProperExtension();
            }
        }

        string workingDir = @"C:\Users\" + System.Environment.UserName + @"\Documents\Visual Studio 2015\Projects\BackgroundImageSorter\test_images\special\trouble files";

        [Test]
        public void TestMethod2()
        {

            string pdfInput = System.IO.Path.Combine(workingDir, "PDF Document (124815)");
            System.IO.FileInfo pdfFile = new System.IO.FileInfo(pdfInput);

            MimeDetective.FileType aFileType = MimeDetective.MimeTypes.GetFileType(pdfFile);



            Assert.IsTrue(MimeDetective.Extension.Documents.DocumentExtensions.IsPdf(new System.IO.FileInfo(pdfInput)));

            MimeDetective.FileType fileType = MimeDetective.MimeDetective.LearnMimeType(new System.IO.FileInfo(pdfInput), "application/pdf-1-4", 8);
            fileType.Extension = "pdf";
            //input1;

            //string[] problemFiles = System.IO.Directory.GetFiles(workingDir);
            string fileName = "FILE0089.CHK";
            string detectedExtension1 = detectExtensionTemplate(fileName);

            //Assert.AreEqual(detectedExtension1, "bill");

            Assert.AreEqual(detectExtensionTemplate("PDF Document (124815)"), "pdf");
            Assert.AreEqual(detectExtensionTemplate("Zip Archive (269333)"), "zip");
        }

        private string detectExtensionTemplate(string fileName)
        {
            string input1 = System.IO.Path.Combine(workingDir, fileName);

            string detectedExtension1 = FileUtilities.DetectProperExtension(input1);
            return detectedExtension1;
        }
    }
}