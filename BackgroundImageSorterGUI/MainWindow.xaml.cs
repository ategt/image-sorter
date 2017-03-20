using BackgroundImageSorter;
using BackgroundImageSorter.Controller;
using BackgroundImageSorter.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

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
    }
}
