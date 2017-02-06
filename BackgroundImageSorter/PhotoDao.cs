using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;

namespace BackgroundImageSorter
{
    public class PhotoDao
    {

        private string photoFilepath;
        IList<Photo> photos = new List<Photo>();
        int nextId = 1;

        public PhotoDao(string photoFilePath= @"PhotoFile.bin")
        {
            this.photoFilepath = photoFilePath;
            photos = decode();

            nextId = DetermineNextId(photos);
        }

        public int DetermineNextId(IList<Photo> PhotoList)
        {
            if (PhotoList == null) return 1;
            else if (PhotoList.Count == 1) return PhotoList.First().Id++;
            else if (PhotoList.Count > 1)
            {
                int highestId = PhotoList.Max(s => s.Id);
                return highestId++;
            }
            return 1;
        }

        public Photo Create(Photo Photo)
        {
            if (Contains(Photo)) return null;

            Photo.Id = nextId;
            nextId++;
            photos.Add(Photo);
            encode();
            return Photo;
        }

        public Photo Get(int id)
        {
            return photos.First(Photo => Photo.Id == id);
        }

        public void Update(Photo Photo)
        {
            Photo foundPhoto = Get(Photo.Id);

            photos.Remove(foundPhoto);
            photos.Add(Photo);

            encode();
        }

        public void Delete(Photo Photo)
        {
            Photo foundPhoto = Get(Photo.Id);

            photos.Remove(foundPhoto);

            encode();
        }

        public IList<Photo> GetList()
        {
            return new List<Photo>(photos);
        }

        private void encode()
        {
            //using (FileStream binaryStream = File.OpenWrite(photoFilepath))
            //using (FileStream binaryStream = File.Create( (photoFilepath), 1024, FileOptions.)
            using (Stream binaryStream = new FileStream(photoFilepath, FileMode.Create, FileAccess.Write, FileShare.None))
            {
                BinaryFormatter binaryFormatter = new BinaryFormatter();
                binaryFormatter.Serialize(binaryStream, photos);
            }
        }

        private IList<Photo> decode()
        {
            if (!File.Exists(photoFilepath)) return new List<Photo>();

            IList<Photo> Photos;

            //using (FileStream binaryStream = File.Open(photoFilepath, FileMode.OpenOrCreate))
            using (Stream binaryStream = new FileStream(photoFilepath, FileMode.Open, FileAccess.Read, FileShare.Read))
            {
                BinaryFormatter binaryFormatter = new BinaryFormatter();
                Photos = (List<Photo>)binaryFormatter.Deserialize(binaryStream);
            }
            return Photos;
        }

        public int size()
        {
            return photos.Count;
        }

        public int Images()
        {
            return photos.Count(photo => !photo.Dimension.IsEmpty);
        }

        public int Backgrounds()
        {
            return photos.Select(photo => photo.Dimension)
                .Where(dimension => !dimension.IsEmpty)
                .Count(dimension => dimension.Height >= 1080 && dimension.Width >= 1080);
        }

        public bool ContainsFile(FileInfo fileInfo)
        {
            return GetByFile(fileInfo) != null;

        }

        public bool Contains(Photo Photo)
        {
            Photo foundPhoto = GetBySHA512(Photo.SHA512);

            if (foundPhoto == null) return false;

            return (foundPhoto.Digest.SequenceEqual(Photo.Digest)) &&
                (foundPhoto.SHA256.SequenceEqual(Photo.SHA256));
        }

        public bool ContainsMD5(byte[] digest)
        {
            return GetByMD5(digest) != null;
        }

        public Photo GetByMD5(byte[] digest)
        {
            return photos.Where(testPhoto => testPhoto.Digest.SequenceEqual(digest)).SingleOrDefault();
        }

        public Photo GetByFile(FileInfo fileInfo)
        {
            string baseName = fileInfo.FullName;

            foreach (Photo Photo in photos)
            {
                string str1 = Photo.FileInfo.FullName;
                bool same = baseName.Equals(str1, System.StringComparison.OrdinalIgnoreCase);
            }

            return photos.Where(testPhoto => testPhoto
                                                    .FileInfo
                                                    .FullName
                                                    .Equals(fileInfo.FullName,
                                                            System.StringComparison
                                                                    .OrdinalIgnoreCase)

                                                    ).SingleOrDefault();
        }

        public bool ContainsSHA256(byte[] digest)
        {
            return GetBySHA256(digest) != null;
        }

        public Photo GetBySHA256(byte[] digest)
        {
            return photos.Where(testPhoto => testPhoto.SHA256.SequenceEqual(digest)).SingleOrDefault();
        }
        public bool ContainsSHA512(byte[] digest)
        {
            return GetBySHA512(digest) != null;
        }

        public Photo GetBySHA512(byte[] digest)
        {
            return photos.Where(testPhoto => testPhoto.SHA512.SequenceEqual(digest)).SingleOrDefault();
        }

    }
}
