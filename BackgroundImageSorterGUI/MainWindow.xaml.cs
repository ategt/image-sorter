using BackgroundImageSorter;
using BackgroundImageSorter.Controller;
using BackgroundImageSorter.Model;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Forms;
using System.Windows.Media;

namespace BackgroundImageSorterGUI
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        ApplicationController applicationController = null;
        Configuration config = null;
        ConfigurationController configurationController = null;
        PhotoViewModel photoViewModel = null;
        BackgroundImageSorter.View.IView consoleView = null;

        public MainWindow()
        {
            InitializeComponent();

            string workingDir = @"C:\Users\" + Environment.UserName + @"\Documents\Visual Studio 2015\Projects\BackgroundImageSorter\test_images";
            //string[] args = new string[] { "-d", workingDir + @"\data.bin",
            //                        @"--s=" + workingDir + @"\input",
            //                        @"/Output:" + workingDir + @"\output" };

            consoleView = new BackgroundImageSorter.View.WindowView(progressBar, this.Dispatcher);
            configurationController = new ConfigurationController(consoleView);

            //consoleView.pr progressBar;

            //config = configurationController.SetupConfiguration(args, ConfigurationBuilder.BuildConfig());
            config = new Configuration
            {
                DataFile = new System.IO.FileInfo(workingDir + @"\data.bin"),
                Source = new System.IO.DirectoryInfo(workingDir + @"\input"),
                Destination = new System.IO.DirectoryInfo(workingDir + @"\output"),
                Recurse = true,
                FastScan = true
            };

            config = configurationController.SetDefaultDirectories(config);

            applicationController = new ApplicationController(new BackgroundImageSorter.Controller.IOController(consoleView), consoleView, new PhotoDao(), configurationController);

            OnConfigChange();
        }

        private async void CheckForUniquePhotos_Click(object sender, RoutedEventArgs e)
        {
            //UpdateProgressBar();

            //uniquePhotoThread();
            //await uniquePhotoThreadAsync();
            await Task.Run(() => uniquePhotoThread());
            //DataContextChanged(sender, e);
        }

        private PhotoViewModel uniquePhotoThread()
        {
            IEnumerable<Photo> uniquePhotos = null;
            this.Dispatcher.Invoke(() =>
            {
                configurationController.ConfirmImportantFoldersExist(config);
                uniquePhotos = applicationController.FindUniquePhotos(config, new Report());
            });
            photoViewModel = new PhotoViewModel();

            foreach (Photo photo in uniquePhotos)
            {
                photoViewModel.Photos.Add(photo);
            }

            //DataContext = await
            this.Dispatcher.Invoke(() =>
            {
                DataContext = photoViewModel;
            });
            return photoViewModel;            
        }

        private async void uniquePhotoThreadAsync()
        {
            // This method runs asynchronously.

            await Task.Run(() => uniquePhotoThread());           
        }


        private void UpdateProgressBar()
        {
            BackgroundWorker worker = new BackgroundWorker();
            worker.WorkerReportsProgress = true;
            worker.DoWork += worker_DoWork;
            worker.ProgressChanged += worker_ProgressChanged;

            worker.RunWorkerAsync();
        }

        private void ChooseDatabaseFile_Click(object sender, RoutedEventArgs e)
        {
            SaveDatabaseByDialog(config.DataFile.Directory.FullName);
        }

        private void SaveDatabaseByDialog(string initialDirectory)
        {
            Microsoft.Win32.SaveFileDialog fileDialog = new Microsoft.Win32.SaveFileDialog();
            fileDialog.FileName = config.DataFile.Name;
            fileDialog.InitialDirectory = initialDirectory;
            fileDialog.DefaultExt = ".bin";
            fileDialog.Filter = "Binary Data File (.bin)|*.bin";

            Nullable<bool> result = fileDialog.ShowDialog();

            if (result == true)
            {
                config.DataFile = new System.IO.FileInfo(fileDialog.FileName);
            }
        }

        private void ChooseInputFolder_Click(object sender, RoutedEventArgs e)
        {
            Microsoft.Win32.SaveFileDialog fileDialog = new Microsoft.Win32.SaveFileDialog();
            fileDialog.FileName = config.Source.Name;
            fileDialog.InitialDirectory = config.Source.FullName;

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
            folderBrowserDialog1.SelectedPath = config.Source.FullName;

            DialogResult result = folderBrowserDialog1.ShowDialog();

            if (System.Windows.Forms.DialogResult.OK == result)
            {
                config.Source = new System.IO.DirectoryInfo(folderBrowserDialog1.SelectedPath);
                OnConfigChange();
            }
        }

        private void ChooseDatabaseFile_Click_1(object sender, RoutedEventArgs e)
        {
            Microsoft.Win32.SaveFileDialog fileDialog = new Microsoft.Win32.SaveFileDialog();
            fileDialog.FileName = config.DataFile.Name;
            fileDialog.InitialDirectory = config.DataFile.DirectoryName;

            Nullable<bool> result = fileDialog.ShowDialog();

            if (result == true)
            {
                config.DataFile = new System.IO.FileInfo(fileDialog.FileName);
                OnConfigChange();
            }
        }

        private void OnConfigChange()
        {
            databaseFile.Text = config.DataFile.FullName;
            destinationPath.Text = config.Destination.FullName;
            sourcePath.Text = config.Source.FullName;
        }

        private void ChooseDestinationFolder_Click(object sender, RoutedEventArgs e)
        {
            System.Windows.Forms.FolderBrowserDialog folderBrowserDialog1 = new System.Windows.Forms.FolderBrowserDialog();

            folderBrowserDialog1.Description =
                "Select the directory that you want to use as the destination.";

            folderBrowserDialog1.ShowNewFolderButton = true;

            folderBrowserDialog1.RootFolder = Environment.SpecialFolder.Desktop;
            folderBrowserDialog1.SelectedPath = config.Destination.FullName;

            DialogResult result = folderBrowserDialog1.ShowDialog();

            if (System.Windows.Forms.DialogResult.OK == result)
            {
                config.Destination = new System.IO.DirectoryInfo(folderBrowserDialog1.SelectedPath);
                OnConfigChange();
            }
        }

        //private void Image_MouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        //{
        //    image1.Source = photoViewModel.RandomImage();
        //}

        //private void TextBlock_MouseEnter(object sender, System.Windows.Input.MouseEventArgs e)
        //{
        //    System.Windows.FrameworkElement element = (System.Windows.FrameworkElement)sender;
        //    object dCtx = element.DataContext;

        //    if (dCtx is Photo)
        //    {
        //        Photo photo = (Photo)dCtx;
        //        Uri uri = new Uri(photo.FileInfo.FullName);
        //        image1.Source = new System.Windows.Media.Imaging.BitmapImage(uri);
        //    }
        //    else
        //        image1.Source = null;
        //}

        private void Grid_Drop(object sender, System.Windows.DragEventArgs e)
        {
            System.Windows.FrameworkElement element = (System.Windows.FrameworkElement)sender;

            if (e.Data.GetDataPresent(System.Windows.DataFormats.FileDrop))
            {
                string[] files = (string[])e.Data.GetData(System.Windows.DataFormats.FileDrop);
                string dropedFile = files[0];
                string target = (string)element.Tag;
                switch (target)
                {
                    case "source":
                        config.Source = new System.IO.DirectoryInfo(dropedFile);
                        break;
                    case "destination":
                        config.Destination = new System.IO.DirectoryInfo(dropedFile);
                        break;
                    case "database":
                        if (System.IO.Directory.Exists(dropedFile))
                            SaveDatabaseByDialog(dropedFile);
                        else
                            config.DataFile = new System.IO.FileInfo(dropedFile);

                        break;
                    default:
                        break;
                }
                OnConfigChange();

            }
        }

        private void sourcePath_PreviewDragOver(object sender, System.Windows.DragEventArgs e)
        {
            if (e.Data.GetDataPresent(System.Windows.DataFormats.FileDrop))
            {
                string[] files = (string[])e.Data.GetData(System.Windows.DataFormats.FileDrop);
                if (System.IO.Directory.Exists(files[0]))
                {
                    e.Handled = true;
                }
            }
        }

        private void databaseFile_PreviewDragOver(object sender, System.Windows.DragEventArgs e)
        {
            e.Handled = true;
        }

        private void Grid_PreviewMouseWheel(object sender, System.Windows.Input.MouseWheelEventArgs e)
        {
            ScrollViewer scv = (ScrollViewer)sender;

            scv.ScrollToVerticalOffset(scv.VerticalOffset - e.Delta);
            e.Handled = true;
        }

        void worker_DoWork(object sender, DoWorkEventArgs e)
        {
            uniquePhotoThread();
            //for (int i = 0; i < 100; i++)
            //{
            //    (sender as BackgroundWorker).ReportProgress(i);
            //    //Thread.Sleep(100);
            //}
        }

        void worker_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            progressBar.Value = e.ProgressPercentage;
        }
    }
}
