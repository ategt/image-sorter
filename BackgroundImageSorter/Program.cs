using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BackgroundImageSorter
{
    class Program
    {
        static void Main(string[] args)
        {
            System.Drawing.Image image = System.Drawing.Image.FromFile(@"C:\Users\ATeg\Desktop\test_images\20151105_090845.jpg");
            System.Drawing.Size imageSize = image.Size;
            //MessageBox.Show("Width: " + );
            Console.WriteLine($"Width: {imageSize.Width}, Height: {imageSize.Height}");
        }
    }
}
