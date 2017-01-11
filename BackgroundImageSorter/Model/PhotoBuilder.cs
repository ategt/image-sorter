using System.IO;
using System.Security.Cryptography;

namespace BackgroundImageSorter.Model
{
    class PhotoBuilder
    {

        public static Photo Build(string path)
        {

            Photo photo = new Photo();

            photo.Digest = GetMD5Hash(path);
            photo.SHA512 = GetSHA512Hash(path);
            photo.SHA256 = GetSHA256Hash(path);
            //photo.Path = path;
            photo.FileInfo = new FileInfo(path);
            photo.Size = photo.FileInfo.Length;

            return photo;

        }

        public static byte[] GetSHA512Hash(string path)
        {
            byte[] hash;

            using (FileStream stream = File.OpenRead(path))
            {
                SHA512Managed sha = new SHA512Managed();
                hash = sha.ComputeHash(stream);
            }

            return hash;
        }
        public static byte[] GetSHA256Hash(string path)
        {
            byte[] hash;

            using (FileStream stream = File.OpenRead(path))
            {
                SHA256Managed sha = new SHA256Managed();
                hash = sha.ComputeHash(stream);
            }

            return hash;
        }
        public static byte[] GetMD5Hash(string path)
        {
            byte[] hash;

            using (FileStream stream = File.OpenRead(path))
            {
                hash = MD5.Create().ComputeHash(stream);
            }

            return hash;
        }

    }
}
