using BackgroundImageSorter.Controller;
using BackgroundImageSorter.Model;
using NDesk.Options;
using System;
using System.Collections.Generic;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;

namespace BackgroundImageSorter
{
    public class Program
    {
        public static void Main(string[] args)
        {
            View.IView consoleView = new View.ConsoleView();
            ConfigurationController configurationController = new ConfigurationController(consoleView);
            Controller.ApplicationController.Program(args, new Controller.IOController(consoleView), consoleView, new PhotoDao(), configurationController);
        }
    }
}
