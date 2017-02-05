using BackgroundImageSorter.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BackgroundImageSorter.View
{
    class ConsoleView
    {
        public static void PrintReport(Report report)
        {
            if (report == null) return;

            Console.WriteLine();

            Console.WriteLine(report);

            Console.WriteLine("Press Any Key to exit...");
            while (!Console.KeyAvailable) { }
        }

    }
}
