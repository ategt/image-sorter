using System.IO;
using BackgroundImageSorter.Model;
using NDesk.Options;

namespace BackgroundImageSorter.View
{
    public interface IView
    {
        int TotalFilesToTransfer { get; set; }
        int CurrentFileNumberTransfering { get; set; }

        void DisplayAFileHasBeenAccepted(int accepted, int rejected, int total);
        void DisplayAFileHasBeenFilteredOut(int accepted, int rejected, int total);
        void DisplayBeginingDirectoryPrescan();
        void DisplayBeginTransferingData();
        void DisplayBeginUpdatingDao();
        void DisplayDaoLoadingBeginning();
        void DisplayDaoLoadingFinished();
        void DisplayDaoScanProgress(int currentPosition, int totalPositions);
        void DisplayDirectoryDoesNotExist(DirectoryInfo directory);
        Report DisplayError();
        Report DisplayErrorMessage(OptionException e);
        void DisplayFinishedDirectoryPrescan();
        void DisplayFinishedTransferingData();
        void DisplayFinishedUpdateingDao();
        void DisplaySkippingPhotoDuringTransfer(Photo photo, IOException ex);
        void DisplaySourceScanningBegining();
        void DisplaySourceScanningFinished();
        void PrintReport(Report report);
        void DisplayNoFilesMoved();
        void ShowHelp(OptionSet p);
        void DisplayFileTestTransfer(string inputFullName, string outputFullName);
        void DisplayScanProgress(int currentPosition, int totalPossibles);
        void DisplayCurrentFileTransfer(string destinationFullName = null, int currentFileNumber = 0);
        void DisplayCompleteFileTransfer(string destinationFullName = null);
    }
}