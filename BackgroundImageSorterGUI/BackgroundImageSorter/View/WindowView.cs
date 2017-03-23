using System;
using System.IO;
using BackgroundImageSorter.Model;

namespace BackgroundImageSorter.View
{
    public class WindowView : IView
    {
        private System.Windows.Controls.ProgressBar progressBar = null;
        private System.Windows.Threading.Dispatcher dispatcher = null;

        public WindowView(System.Windows.Controls.ProgressBar progressBar, System.Windows.Threading.Dispatcher dispatcher)
        {
            this.progressBar = progressBar;
            this.dispatcher = dispatcher;
        }

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
            //(sender as BackgroundWorker).ReportProgress(i);
            dispatcher.Invoke(new Action(() =>
            {
                progressBar.Value = (int)(((float)currentPosition / (float)totalPossibles) * (float)progressBar.Maximum);
            }), System.Windows.Threading.DispatcherPriority.ContextIdle);


            //progressBar.Value = (int)(((float)currentPosition / (float)totalPossibles) * (float)progressBar.Maximum);
            //progressBar.
        }

        //void worker_ProgressChanged(object sender, System.ComponentModel.ProgressChangedEventArgs e)
        //{
        //    progressBar.Value = e.ProgressPercentage;
        //}

        public void DisplaySkippingPhotoDuringTransfer(Photo photo, IOException ex)
        {
            // Do nothing.
        }

        public void DisplaySourceScanningBegining()
        {
            progressBar.Value = 5;
        }

        public void DisplaySourceScanningFinished()
        {
            progressBar.Value = 75;
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