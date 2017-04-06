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

            string pdfInput2 = System.IO.Path.Combine(workingDir, "PDF Document (124815)");
            System.IO.FileInfo pdfFile2 = new System.IO.FileInfo(pdfInput2);

            MimeDetective.FileType aFileType2 = MimeDetective.MimeTypes.GetFileType(pdfFile2);

            string pdfInput3 = System.IO.Path.Combine(workingDir, "WinZip Archive (1378)");
            System.IO.FileInfo pdfFile3 = new System.IO.FileInfo(pdfInput3);

            MimeDetective.FileType aFileType3 = MimeDetective.MimeTypes.GetFileType(pdfFile3);

            string pdfInput4 = System.IO.Path.Combine(workingDir, "Zip Archive (269250)");
            System.IO.FileInfo pdfFile4 = new System.IO.FileInfo(pdfInput4);

            MimeDetective.FileType aFileType4 = MimeDetective.MimeTypes.GetFileType(pdfFile4);

            string pdfInput5 = System.IO.Path.Combine(workingDir, "PDF Document (124815)");
            System.IO.FileInfo pdfFile5 = new System.IO.FileInfo(pdfInput5);

            MimeDetective.FileType aFileType5 = MimeDetective.MimeTypes.GetFileType(pdfFile5);

            //MimeDetective.MimeTypes.
            //MimeDetective.MimeTypes.types.Add();
            //Assert.IsTrue(MimeDetective.Extension.Documents.DocumentExtensions.IsPdf(new System.IO.FileInfo(pdfInput)));

            MimeDetective.FileType newFileType = MimeDetective.MimeDetective.LearnMimeType(pdfFile3, pdfFile4, "application/pkzip");
            newFileType.Extension = "zip";
            MimeDetective.MimeTypes.types.Add(newFileType);
            //MimeDetective.FileType newFileType = MimeDetective.MimeDetective.LearnMimeType(new System.IO.FileInfo(pdfInput), "application/pdf-1-4", 8);
            //fileType.Extension = "pdf";
            //input1;

            //string[] problemFiles = System.IO.Directory.GetFiles(workingDir);
            string fileName = "FILE0089.CHK";
            string detectedExtension1 = detectExtensionTemplate(fileName);

            //Assert.AreEqual(detectedExtension1, "bill");

            Assert.AreEqual(detectExtensionTemplate("PDF Document (124815)"), "pdf");
            Assert.AreEqual(detectExtensionTemplate("Zip Archive (269333)"), "zip");

            Assert.AreEqual(detectExtensionByFile("PDF Document (124815)"), "pdf");
            Assert.AreEqual(detectExtensionByFile("Zip Archive (269333)"), "zip");


            Assert.AreEqual(detectExtensionByFile("16464157_924752977628291_4243522177728512000_n.jpg"), "jpg");
            Assert.AreEqual(detectExtensionByFile("download.png"), "png");
            Assert.AreEqual(detectExtensionByFile("FILE0089.CHK"), null);
            Assert.AreEqual(detectExtensionByFile("FILE6573.CHK"), null);
            Assert.AreEqual(detectExtensionByFile("files.txt"), null);
            Assert.AreEqual(detectExtensionByFile("libsox.pdf"), "pdf");
            Assert.AreEqual(detectExtensionByFile("PDF Document(121860)"), "pdf");
            Assert.AreEqual(detectExtensionByFile("PDF Document (123704)"), "pdf");
            Assert.AreEqual(detectExtensionByFile("PDF Document(124815)"), "pdf");
            Assert.AreEqual(detectExtensionByFile("soxi.pdf"), "pdf");
            Assert.AreEqual(detectExtensionByFile("WinZip Archive(1378)"), "zip");
            Assert.AreEqual(detectExtensionByFile("Zip Archive(269250)"), "zip");
            Assert.AreEqual(detectExtensionByFile("Zip Archive(269333)"), "zip");

        }

        private string detectExtensionTemplate(string fileName)
        {
            string input1 = System.IO.Path.Combine(workingDir, fileName);

            System.IO.FileInfo inputFile = new System.IO.FileInfo(input1);

            byte[] inputData = System.IO.File.ReadAllBytes(inputFile.FullName);
            MimeDetective.FileType aFileType = MimeDetective.MimeTypes.GetFileType(() => inputData);
            //MimeDetective.FileType aFileType = MimeDetective.MimeTypes.GetFileType(inputFile);

            //string detectedExtension1 = FileUtilities.DetectProperExtension(input1);
            string detectedExtension1 = aFileType?.Extension;
            return detectedExtension1;
        }

        private string detectExtensionByFile(string fileName)
        {
            string input1 = System.IO.Path.Combine(workingDir, fileName);

            System.IO.FileInfo inputFile = new System.IO.FileInfo(input1);
            MimeDetective.FileType aFileType = MimeDetective.MimeTypes.GetFileType(inputFile);
            string detectedExtension1 = aFileType?.Extension;
            return detectedExtension1;
        }
    }
}