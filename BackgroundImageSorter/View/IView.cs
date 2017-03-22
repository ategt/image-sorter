using System.IO;
using BackgroundImageSorter.Model;
using NDesk.Options;

namespace BackgroundImageSorter.View
{
    public interface IView
    {
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
        void ShowHelp(OptionSet p);
    }
}