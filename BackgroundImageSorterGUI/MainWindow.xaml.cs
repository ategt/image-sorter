using BackgroundImageSorter;
using BackgroundImageSorter.Controller;
using BackgroundImageSorter.Model;
using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Forms;

namespace BackgroundImageSorterGUI
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        ApplicationController applicationController = null;
        Configuration config = null;

        public MainWindow()
        {
            string workingDir = @"C:\Users\" + Environment.UserName + @"\Documents\Visual Studio 2015\Projects\BackgroundImageSorter\test_images";
            string[] args = new string[] { "-d", workingDir + @"\data.bin",
                                    @"--s=" + workingDir + @"\input",
                                    @"/Output:" + workingDir + @"\output" };

            config = ConfigurationController.SetupConfiguration(args, ConfigurationBuilder.BuildConfig());

            InitializeComponent();
        }

        private void CheckForUniquePhotos_Click(object sender, RoutedEventArgs e)
        {
            IEnumerable<Photo> uniquePhotos = applicationController.FindUniquePhotos(config, new Report());

        }

        private void ChooseDatabaseFile_Click(object sender, RoutedEventArgs e)
        {
            Microsoft.Win32.SaveFileDialog fileDialog = new Microsoft.Win32.SaveFileDialog();
            //fileDialog.FileName = "database.bin";
            fileDialog.FileName = config.DataFile.Name;
            fileDialog.InitialDirectory = config.DataFile.Directory.FullName;
            fileDialog.DefaultExt = ".bin";
            fileDialog.Filter = "Binary Data File (*.bin)|*.bin";

            Nullable<bool> result = fileDialog.ShowDialog();

            if (result== true)
            {
                config.DataFile = new System.IO.FileInfo(fileDialog.FileName);
            }
        }

        private void ChooseInputFolder_Click(object sender, RoutedEventArgs e)
        {
            Microsoft.Win32.SaveFileDialog fileDialog = new Microsoft.Win32.SaveFileDialog();
            //fileDialog.FileName = "database.bin";
            fileDialog.FileName = config.Source.FullName;
            fileDialog.InitialDirectory = config.Source.FullName;
            //fileDialog.
            //fileDialog.Filter = "Binary Data File (*.bin)|*.bin";

            Nullable<bool> result = fileDialog.ShowDialog();

            if (result == true)
            {
                config.Source = new System.IO.DirectoryInfo(fileDialog.FileName);
            }
        }

        private void SourceButton_Click(object sender, RoutedEventArgs e)
        {
            System.Windows.Forms.FolderBrowserDialog folderBrowserDialog1 = new System.Windows.Forms.FolderBrowserDialog();

            // Set the help text description for the FolderBrowserDialog.
            folderBrowserDialog1.Description =
                "Select the directory that you want to use as the default.";

            // Do not allow the user to create new files via the FolderBrowserDialog.
            folderBrowserDialog1.ShowNewFolderButton = true;

            // Default to the My Documents folder.
            folderBrowserDialog1.RootFolder = Environment.SpecialFolder.Desktop;
            folderBrowserDialog1.SelectedPath = Environment.GetFolderPath( Environment.SpecialFolder.DesktopDirectory);

            DialogResult result = folderBrowserDialog1.ShowDialog();

            if (result.Equals(true))
            {
                config.Source = new System.IO.DirectoryInfo(folderBrowserDialog1.SelectedPath);
                //if (!fileOpened)
                //{
                //    // No file is opened, bring up openFileDialog in selected path.
                //    openFileDialog1.InitialDirectory = folderName;
                //    openFileDialog1.FileName = null;
                //    openMenuItem.PerformClick();
                //}
            }
        }

        private void ChooseDatabaseFile_Click_1(object sender, RoutedEventArgs e)
        {
            FolderBrowserDialogExampleForm.Launch();
        }
    }
}
