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
            Controller.ApplicationController.Program(args);
        }
    }
}
