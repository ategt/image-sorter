using System;
using System.Linq;

namespace BackgroundImageSorter
{
    [System.Serializable]
    public class Photo : IEquatable<Photo>
    {
        public int Id { get; set; }
        public byte[] Digest { get; set; }
        public byte[] SHA512 { get; set; }
        public byte[] SHA256 { get; set; }
        public System.IO.FileInfo FileInfo { get; set; }
        public System.Drawing.Size Dimension { get; set; }
        public System.Guid Format { get; set; }
        public long Size { get; set; }
        public string hash { get; set; }

        //public override bool Equals(object obj)
        //{

        //    if (obj == null || GetType() != obj.GetType())
        //    {
        //        return false;
        //    }

        //    Photo otherPhoto = (Photo)obj;

        //    return
        //        SHA512.SequenceEqual(otherPhoto.SHA512) &&
        //        SHA256.SequenceEqual(otherPhoto.SHA256) &&
        //        Digest.SequenceEqual(otherPhoto.Digest);

        //}

        //public override int GetHashCode()
        //{
        //    return base.GetHashCode();
        //}

        bool IEquatable<Photo>.Equals(Photo other)
        {
            return
               SHA512.SequenceEqual(other.SHA512) &&
               SHA256.SequenceEqual(other.SHA256) &&
               Digest.SequenceEqual(other.Digest);
        }
    }
}
