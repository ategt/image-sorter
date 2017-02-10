using BackgroundImageSorter.Model;
using NDesk.Options;
using System;
using System.IO;

namespace BackgroundImageSorter.View
{
    public class ConsoleView
    {
        public readonly static string BEGINNING_SCANNING_SOURCE = "Scaning Source Directory....";
        public readonly static string BEGINNING_SCANNING_DESTINATION = "Scaning Destination Directory....";

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
        }

        internal void DisplayProgress(int current, int count)
        {
            Console.WriteLine(MakeProgressBar(current, count));
        }

        public static void DisplayFinishedDirectoryPrescan()
        {
            EraseCurrentLine();
            Console.WriteLine($"{BEGINNING_SCANNING_DESTINATION}PreScan Done.");
        }

        public static void DisplayBeginingDirectoryPrescan()
        {
            Console.WriteLine("PreScanning Destination Folder.");
            Console.Write(BEGINNING_SCANNING_DESTINATION);
        }
        
        public void DisplayDaoScanProgress(int currentPosition, int totalPositions)
        {
            string progressBar = MakeProgressBar(currentPosition, totalPositions);

            WriteFromBeginingOfConsoleLine($"{BEGINNING_SCANNING_DESTINATION}{progressBar}");
        }

        public static void DisplayFinishedUpdateingDao()
        {
            EraseCurrentLine();
            Console.WriteLine($"{BEGINNING_SCANNING_DESTINATION}Dao updated.");
        }

        public static void DisplayBeginUpdatingDao()
        {
            Console.WriteLine("Updating File Data.");
            Console.Write(BEGINNING_SCANNING_DESTINATION);
        }

        public static void DisplayFinishedTransferingData()
        {
            Console.WriteLine("Copy Complete.");
        }

        public static void DisplayBeginTransferingData()
        {
            Console.Write("Copying Photos...");
        }

        internal void DisplayNoFilesMoved()
        {
            Console.WriteLine("No Files Were Moved...Skipping Data Update.");
        }

        internal static void DisplayFileTestTransfer(string inputFullName, string outputFullName)
        {
            Console.WriteLine($"{inputFullName} to {outputFullName}");
        }

        public static void DisplaySourceScanningBegining()
        {
            Console.Write(BEGINNING_SCANNING_SOURCE);
        }

        internal static void DisplayScanProgress(int currentPosition, int totalPossibles)
        {
            string progressBar = MakeProgressBar(currentPosition, totalPossibles);

            string output = $"{BEGINNING_SCANNING_SOURCE}{progressBar}";
            WriteFromBeginingOfConsoleLine(output);
        }

        private static void WriteFromBeginingOfConsoleLine(string output)
        {
            Console.CursorLeft = 0;
            Console.Write(output);
        }

        public static void DisplayAFileHasBeenAccepted(int accepted, int rejected, int total)
        {
            DisplayFileFilterUpdate(accepted, rejected, total);
        }

        public static void DisplayAFileHasBeenFilteredOut(int accepted, int rejected, int total)
        {
            DisplayFileFilterUpdate(accepted, rejected, total);
        }

        private static void DisplayFileFilterUpdate(int accepted, int rejected, int total)
        {
            string output = $"{BEGINNING_SCANNING_SOURCE} New:{accepted}\t Already Have:{rejected}\t Total:{total}";
            WriteFromBeginingOfConsoleLine(output);
        }

        public static void DisplaySourceScanningFinished()
        {
            EraseCurrentLine();
            Console.WriteLine($"{BEGINNING_SCANNING_SOURCE}Complete.");
        }

        private static void EraseCurrentLine()
        {
            Console.CursorLeft = 0;
            Console.Write(new string(' ', Console.WindowWidth - 2));
            Console.CursorLeft = 0;
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

        public static void DisplayDirectoryDoesNotExist(DirectoryInfo directory)
        {
            Console.WriteLine(directory.FullName + " does not exist.");
        }

        public static void DisplaySkippingPhotoDuringTransfer(Photo photo, IOException ex)
        {
            Console.WriteLine("Skipping " + photo.FileInfo.Name);
            Console.WriteLine(ex.Message);
        }

        private static string MakeProgressBar(int currentPosition, int totalPossibles)
        {
            int lengthOfBar = 20;
            int dotsToShow = (int)(((float)currentPosition / (float)totalPossibles) * (float)lengthOfBar);
            if (dotsToShow > 20) dotsToShow = 20;
            int spacesToShow = lengthOfBar - dotsToShow;

            string dots = new string('-', dotsToShow);
            string spaceLeftOver = new string(' ', spacesToShow);
            string progressBar = $"[{dots}{spaceLeftOver}]";

            string output = $"{progressBar}({currentPosition}/{totalPossibles})";
            return output;
        }
    }
}
