using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BackgroundImageSorter
{
    class PhotoDao
    {

        List<Photo> myPhoto = new List<Photo>();
        string path = "Photo.txt";
        int nextId = 1;
        string TOKEN = "::;:";
        string NEW_LINE = Environment.NewLine;


        public PhotoDao()
        {
            myPhoto = Decode();
            DetermineNextId(myPhoto);
        }

        public void DetermineNextId(List<Photo> PhotoList)
        {
            if (PhotoList == null)
            { }
            else if (PhotoList.Count == 1) nextId = PhotoList.First().Id++;
            else if (PhotoList.Count > 1)
            {
                int highestId = PhotoList.Max(s => s.Id);
                nextId = highestId++;
            }
        }

        public Photo Create(Photo Photo)
        {
            Photo.Id = nextId;
            nextId++;
            myPhoto.Add(Photo);
            Encode();
            return Photo;
        }

        public Photo Get(int id)
        {
            return myPhoto.Find(Photo => Photo.Id == id);
        }

        public void Update(Photo Photo)
        {
            Photo foundPhoto = Get(Photo.Id);

            myPhoto.Remove(foundPhoto);
            myPhoto.Add(Photo);

            Encode();
        }

        public void Delete(Photo Photo)
        {
            Photo foundPhoto = Get(Photo.Id);

            myPhoto.Remove(foundPhoto);

            Encode();
        }

        public List<Photo> GetList()
        {
            List<Photo> cloneList = new List<Photo>();
            myPhoto.ForEach(cloneList.Add);
            return cloneList;
        }

        public void Encode()
        {
            WriteToFile(path, myPhoto);
        }

        private List<Photo> Decode()
        {
            return BuildPhotoFromFile(path);
        }

        public void WriteToFile(string path, List<Photo> myPhoto)
        {
            using (StreamWriter sw = new StreamWriter(path))
            {
                foreach (Photo photo in myPhoto)
                {
                    sw.WriteLine(stringifyPhoto(photo, TOKEN, NEW_LINE));
                }
                sw.Close();
            }
        }

        private String stringifyPhoto(Photo photo, string TOKEN, string NEW_LINE)
        {

            int id = photo.Id;
            byte[] digest = photo.Digest;
            byte[] SHA512 = photo.SHA512;
            byte[] SHA256 = photo.SHA256;
            string fileInfo = photo.FileInfo;
            Size dimension = photo.Dimension;
            long size = photo.Size;
            string path = photo.Path;

            StringBuilder output = new StringBuilder();
            output.Append(id);
            output.Append(TOKEN);

            String digestStr = byteToString(digest);

            output.Append(digestStr);
            output.Append(TOKEN);

            output.Append(byteToString(SHA512));
            output.Append(TOKEN);

            output.Append(byteToString(SHA256));
            output.Append(TOKEN);

            output.Append(fileInfo);
            output.Append(TOKEN);

            long width = (dimension != null) ? dimension.Width : 0;
            output.Append(width);
            output.Append(TOKEN);

            long height = (dimension != null) ? dimension.Height : 0;
            output.Append(height);
            output.Append(TOKEN);

            output.Append(size);
            output.Append(TOKEN);

            output.Append(path);
            output.Append(NEW_LINE);

            return output.ToString();
        }

        private String byteToString(byte[] digest)
        {
            String digestStr = "";
            foreach (byte digestByte in digest)
            {
                digestStr += $"{digestByte.ToString("G")}xx";
            }
            return digestStr;
        }

        public void WriteToLog(Photo myPhoto)
        {

            string path = "Photo-log.txt";

            using (StreamWriter sw = new StreamWriter(path, true))
            {
                //foreach (Photo Photo in myPhoto)
                Photo photo = myPhoto;
                {
                    sw.WriteLine(stringifyPhoto(photo, TOKEN, NEW_LINE));
                }
                sw.Close();
            }
        }



        public List<Photo> BuildPhotoFromFile(string path)
        {

            List<Photo> myPhoto = new List<Photo>();
            string[] result = new string[2];
            if (path == null)
                path = "PhotoText.txt";
            string[] seperator = { TOKEN };
            string[] lineSeperator = { NEW_LINE };

            string fileContent = ReadFile(path);

            if (fileContent == null)
            {
                return myPhoto;
            }
            else
            {
                string[] fileLines = fileContent.Split(lineSeperator, StringSplitOptions.RemoveEmptyEntries);

                foreach (string line in fileLines)
                {
                    result = line.Split(seperator, StringSplitOptions.RemoveEmptyEntries);

                    Photo photo = PhotoBuilder(result[0], result[1]);
                    myPhoto.Add(Photo);
                }

            }

            return myPhoto;
        }

        private Photo unStringifyPhoto(string photoStr, string TOKEN)
        {

            string[] tokenArray = { TOKEN };

            String[] item = photoStr.Split(tokenArray, StringSplitOptions.RemoveEmptyEntries);

                int id = int.Parse(item[0]);

                String byteArrayStr = item[1];
                byte[] byteAr = byteStringToArray(byteArrayStr);

                byte[] digest = byteAr;
                byte[] SHA512 = byteStringToArray(item[2]);
                byte[] SHA256 = byteStringToArray(item[3]);

                File file = new File(item[4]);
                Dimension dimension = new Dimension(Integer.parseInt(item[5]),
                        Integer.parseInt(item[6]));

                long size = Long.parseLong(item[7]);
                int zvidHash = Integer.parseInt(item[8]);

                Thumb thumb = new Thumb();

                thumb.setId(id);
                thumb.setDigest(digest);
                thumb.setSHA512(SHA512);
                thumb.setSHA256(SHA256);
                thumb.setFile(file);
                thumb.setDimension(dimension);
                thumb.setSize(size);
                thumb.setZvidHash(zvidHash);

            return thumb;

            }

        public string ReadFile(string path)
        {

            string defaultFilePath = "PhotoText.txt";
            string filepath = "";

            if (path == null)
            {
                filepath = defaultFilePath;
            }
            else
            {
                filepath = path;
            }

            string content = "";


            if (File.Exists(filepath))
            {
                using (StreamReader sr = new StreamReader(filepath))
                {
                    while (!sr.EndOfStream)
                    {
                        content += sr.ReadLine() + NEW_LINE;
                    }

                    sr.Close();
                }
            }
            else
            {
                return null;
            }

            return content;
        }
    }
}
}
