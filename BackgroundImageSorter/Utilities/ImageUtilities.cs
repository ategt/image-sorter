using System;
using System.Drawing.Imaging;

namespace BackgroundImageSorter.Utilities
{
    public class ImageUtilities
    {        
        public static string DetectProperExtension(Photo photo)
        {
            string properExtension = string.Empty;
            Guid format = photo.Format;
            if (format.Equals(ImageFormat.Jpeg.Guid))
            {
                properExtension = "jpg";
            }
            else if (format.Equals(ImageFormat.Bmp.Guid))
            {
                properExtension = "bmp";
            }
            else if (format.Equals(ImageFormat.Exif.Guid))
            {
                properExtension = "exif";
            }
            else if (format.Equals(ImageFormat.Emf.Guid))
            {
                properExtension = "emf";
            }
            else if (format.Equals(ImageFormat.Gif.Guid))
            {
                properExtension = "gif";
            }
            else if (format.Equals(ImageFormat.Icon.Guid))
            {
                properExtension = "icon";
            }
            else if (format.Equals(ImageFormat.MemoryBmp.Guid))
            {
                properExtension = "mbmp";
            }
            else if (format.Equals(ImageFormat.Png.Guid))
            {
                properExtension = "png";
            }
            else if (format.Equals(ImageFormat.Tiff.Guid))
            {
                properExtension = "tiff";
            }
            else if (format.Equals(ImageFormat.Wmf.Guid))
            {
                properExtension = "wmf";
            }
            else
            {
                properExtension = string.Empty;
            }

            return properExtension;
        }

        public static bool IsImage(Uri uri)
        {
            System.Drawing.Size imageSize = new System.Drawing.Size();

            try
            {
                using (System.Drawing.Image image = System.Drawing.Image.FromFile(uri.LocalPath))
                {
                    imageSize = image.Size;
                    //format = image.RawFormat.Guid;
                    image.Dispose();
                }
            }
            catch (System.OutOfMemoryException ex)
            {
                //imageSize = new System.Drawing.Size();
                //format = System.Guid.Empty;
            }

            return imageSize.IsEmpty;
        }
    }
}
