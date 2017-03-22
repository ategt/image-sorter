using System;
using System.IO;
using BackgroundImageSorter.Model;

namespace BackgroundImageSorter.View
{
    public class NullView : IView
    {
        public void DisplayAFileHasBeenAccepted(int accepted, int rejected, int total)
        {
            // Do nothing.
        }

        public void DisplayAFileHasBeenFilteredOut(int accepted, int rejected, int total)
        {
            // Do nothing.
        }

        public void DisplayBeginingDirectoryPrescan()
        {
            // Do nothing.
        }

        public void DisplayBeginTransferingData()
        {
            // Do nothing.
        }

        public void DisplayBeginUpdatingDao()
        {
            // Do nothing.
        }

        public void DisplayDaoLoadingBeginning()
        {
            // Do nothing.
        }

        public void DisplayDaoLoadingFinished()
        {
            // Do nothing.
        }

        public void DisplayDaoScanProgress(int currentPosition, int totalPositions)
        {
            // Do nothing.
        }

        public void DisplayDirectoryDoesNotExist(DirectoryInfo directory)
        {
            // Do nothing.
        }

        public Report DisplayError()
        {
            // Do nothing.
            return null;
        }

        public Report DisplayErrorMessage(global::NDesk.Options.OptionException e)
        {
            // Do nothing.
            return null;
        }

        public void DisplayFileTestTransfer(string inputFullName, string outputFullName)
        {
            // Do nothing.
        }

        public void DisplayFinishedDirectoryPrescan()
        {
            // Do nothing.
        }

        public void DisplayFinishedTransferingData()
        {
            // Do nothing.
        }

        public void DisplayFinishedUpdateingDao()
        {
            // Do nothing.
        }

        public void DisplayNoFilesMoved()
        {
            // Do nothing.
        }

        public void DisplayScanProgress(int currentPosition, int totalPossibles)
        {
            // Do nothing.
        }

        public void DisplaySkippingPhotoDuringTransfer(Photo photo, IOException ex)
        {
            // Do nothing.
        }

        public void DisplaySourceScanningBegining()
        {
            // Do nothing.
        }

        public void DisplaySourceScanningFinished()
        {
            // Do nothing.
        }

        public void PrintReport(Report report)
        {
            // Do nothing.
        }

        public void ShowHelp(global::NDesk.Options.OptionSet p)
        {
            // Do nothing.
        }
    }
}