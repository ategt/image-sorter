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
            Microsoft.Win32.OpenFileDialog fileDialog = new Microsoft.Win32.OpenFileDialog();
            //fileDialog.FileName = "database.bin";
            fileDialog.FileName = config.DataFile.FullName;
            fileDialog.DefaultExt = ".bin";
            fileDialog.Filter = "Binary Data File (*.bin)|*.bin";

            Nullable<bool> result = fileDialog.ShowDialog();

            if (result== true)
            {
                config.DataFile = new System.IO.FileInfo(fileDialog.FileName);
            }
        }

        public 

        private void ChooseInputFolder_Click(object sender, RoutedEventArgs e)
        {
            System.Windows.
            var dialog = new Microsoft.Win32.CommonDialog(); // CommonOpenFileDialog();
            Microsoft.Win32.OpenFileDialog fileDialog = new Microsoft.Win32.OpenFileDialog();
            //fileDialog.FileName = "database.bin";
            fileDialog.FileName = config.Source.FullName;
            fileDialog.DefaultExt = ".bin";
            fileDialog.Filter = "Binary Data File (*.bin)|*.bin";

            Nullable<bool> result = fileDialog.ShowDialog();

            if (result == true)
            {
                config.DataFile = new System.IO.FileInfo(fileDialog.FileName);
            }

        }
    }
}
