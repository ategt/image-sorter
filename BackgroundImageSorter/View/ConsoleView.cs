using BackgroundImageSorter.Model;
using NDesk.Options;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BackgroundImageSorter.View
{
    class ConsoleView
    {
        public static void PrintReport(Report report)
        {
            if (report == null) return;

            Console.WriteLine();

            Console.WriteLine(report);

            Console.WriteLine("Press Any Key to exit...");
            while (!Console.KeyAvailable) { }
        }


        public static Report DisplayError()
        {
            Console.WriteLine("An important directory is missing.");
            return null;
        }

        public static void ShowHelp(OptionSet p)
        {
            Console.WriteLine("Usage: [OPTIONS]+");
            Console.WriteLine();
            Console.WriteLine("Options:");
            p.WriteOptionDescriptions(Console.Out);
            Console.WriteLine();
            Console.WriteLine("\t Fast Scan Not Implemented Yet.");
        }


        public static void DisplayFinishedDirectoryPrescan()
        {
            Console.WriteLine("PreScan Done.");
        }

        public static void DisplayBeginingDirectoryPrescan()
        {
            Console.Write("PreScanning Destination Folder...");
        }

        public static void DisplayFinishedUpdateingDao()
        {
            Console.WriteLine("Dao updated.");
        }

        public static void DisplayBeginUpdatingDao()
        {
            Console.Write("Updating File Data...");
        }

        public static void DisplayFinishedTransferingData()
        {
            Console.WriteLine("Copy Complete.");
        }

        public static void DisplayBeginTransferingData()
        {
            Console.Write("Copying Photos...");
        }

        public static void DisplaySourceScanningFinished()
        {
            Console.WriteLine("Complete.");
        }

        public static void DisplaySourceScanningBegining()
        {
            Console.Write("Scaning Source Directory...");
        }

        public static void DisplayDaoLoadingFinished()
        {
            Console.WriteLine("Complete.");
        }

        public static void DisplayDaoLoadingBeginning()
        {
            Console.Write("Dao loading...");
        }

        public static Report DisplayErrorMessage(OptionException e)
        {
            Console.Write("BackgroundImageSorter: ");
            Console.WriteLine(e.Message);
            Console.WriteLine("Try `BackgroundImageSorter --help' for more information.");
            return null;
        }

        public static void DisplayAFileHasBeenFilteredOut()
        {
            Console.WriteLine("We Have This One.");
        }

        public static void DisplayDirectoryDoesNotExist(DirectoryInfo directory)
        {
            Console.WriteLine(directory.FullName + " does not exist.");
        }

        public static void DisplaySkippingPhotoDuringTransfer(Photo photo, IOException ex)
        {
            Console.WriteLine("Skipping " + photo.FileInfo.Name);
            Console.WriteLine(ex.Message);
        }

    }
}
