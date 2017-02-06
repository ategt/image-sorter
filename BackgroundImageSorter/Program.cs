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
        static void Main(string[] args)
        {
            View.ConsoleView consoleView = new View.ConsoleView();
            Controller.ApplicationController.Program(args, new Controller.IOController(consoleView), consoleView);
        }
    }
}
