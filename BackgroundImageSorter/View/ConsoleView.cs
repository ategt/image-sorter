using BackgroundImageSorter.Model;
using NDesk.Options;
using System;
using System.IO;

namespace BackgroundImageSorter.View
{
    public class ConsoleView : IView
    {
        public readonly static string BEGINNING_SCANNING_SOURCE = "Scaning Source Directory....";
        public readonly static string BEGINNING_SCANNING_DESTINATION = "Scaning Destination Directory....";
        public readonly static string COPYING_PHOTOS = "Copying Photos...";

        public int TotalFilesToTransfer { get; set; }
        public int CurrentFileNumberTransfering { get; set; }

        public void PrintReport(Report report)
        {
            if (report == null) return;

            Console.WriteLine();

            Console.WriteLine(report);

            Console.WriteLine("Press Any Key to exit...");
            while (!Console.KeyAvailable) { }
        }


        public Report DisplayError()
        {
            Console.WriteLine("An important directory is missing.");
            return null;
        }

        public void ShowHelp(OptionSet p)
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

        public void DisplayFinishedDirectoryPrescan()
        {
            EraseCurrentLine();
            Console.WriteLine($"{BEGINNING_SCANNING_DESTINATION}PreScan Done.");
        }

        public void DisplayBeginingDirectoryPrescan()
        {
            Console.WriteLine("PreScanning Destination Folder.");
            Console.Write(BEGINNING_SCANNING_DESTINATION);
        }

        public void DisplayDaoScanProgress(int currentPosition, int totalPositions)
        {
            string progressBar = MakeProgressBar(currentPosition, totalPositions);

            WriteFromBeginingOfConsoleLine($"{BEGINNING_SCANNING_DESTINATION}{progressBar}");
        }

        public void DisplayFinishedUpdateingDao()
        {
            EraseCurrentLine();
            Console.WriteLine($"{BEGINNING_SCANNING_DESTINATION}Dao updated.");
        }

        public void DisplayBeginUpdatingDao()
        {
            Console.WriteLine("Updating File Data.");
            Console.Write(BEGINNING_SCANNING_DESTINATION);
        }

        public void DisplayFinishedTransferingData()
        {
            WriteFromBeginingOfConsoleLine(COPYING_PHOTOS + "Copy Complete.");
        }

        public void DisplayBeginTransferingData()
        {
            Console.Write(COPYING_PHOTOS);
        }

        public void DisplayNoFilesMoved()
        {
            Console.WriteLine("No Files Were Moved...Skipping Data Update.");
        }

        public void DisplayFileTestTransfer(string inputFullName, string outputFullName)
        {
            Console.WriteLine($"{inputFullName} to {outputFullName}");
        }

        public void DisplaySourceScanningBegining()
        {
            Console.Write(BEGINNING_SCANNING_SOURCE);
        }

        public void DisplayScanProgress(int currentPosition, int totalPossibles)
        {
            string progressBar = MakeProgressBar(currentPosition, totalPossibles);

            string output = $"{BEGINNING_SCANNING_SOURCE}{progressBar}";
            WriteFromBeginingOfConsoleLine(output);
        }

        private void WriteFromBeginingOfConsoleLine(string output)
        {
            Console.CursorLeft = 0;
            Console.Write(output);
        }

        public void DisplayAFileHasBeenAccepted(int accepted, int rejected, int total)
        {
            DisplayFileFilterUpdate(accepted, rejected, total);
        }

        public void DisplayAFileHasBeenFilteredOut(int accepted, int rejected, int total)
        {
            DisplayFileFilterUpdate(accepted, rejected, total);
        }

        private void DisplayFileFilterUpdate(int accepted, int rejected, int total)
        {
            string output = $"{BEGINNING_SCANNING_SOURCE} New:{accepted}\t Already Have:{rejected}\t Total:{total}";
            WriteFromBeginingOfConsoleLine(output);
        }

        public void DisplaySourceScanningFinished()
        {
            EraseCurrentLine();
            Console.WriteLine($"{BEGINNING_SCANNING_SOURCE}Complete.");
        }

        private void EraseCurrentLine()
        {
            Console.CursorLeft = 0;
            Console.Write(new string(' ', Console.WindowWidth - 2));
            Console.CursorLeft = 0;
        }

        public void DisplayDaoLoadingFinished()
        {
            Console.WriteLine("Complete.");
        }

        public void DisplayDaoLoadingBeginning()
        {
            Console.Write("Dao loading...");
        }

        public Report DisplayErrorMessage(OptionException e)
        {
            Console.Write("BackgroundImageSorter: ");
            Console.WriteLine(e.Message);
            Console.WriteLine("Try `BackgroundImageSorter --help' for more information.");
            return null;
        }

        public void DisplayDirectoryDoesNotExist(DirectoryInfo directory)
        {
            Console.WriteLine(directory.FullName + " does not exist.");
        }

        public void DisplaySkippingPhotoDuringTransfer(Photo photo, IOException ex)
        {
            Console.WriteLine("Skipping " + photo.FileInfo.Name);
            Console.WriteLine(ex.Message);
        }

        private string MakeProgressBar(int currentPosition, int totalPossibles)
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

        public void DisplayCurrentFileTransfer(string destinationFullName = null, int currentFileNumber = 0)
        {
            if (currentFileNumber == 0)
                currentFileNumber = CurrentFileNumberTransfering;

            string fileName = Path.GetFileName(destinationFullName);
            WriteFromBeginingOfConsoleLine($"{COPYING_PHOTOS}{MakeProgressBar(currentFileNumber, TotalFilesToTransfer)} - {fileName}");
        }

        public void DisplayCompleteFileTransfer(string destinationFullName = null)
        {
            Console.Write("...Done.");
        }
    }
}
