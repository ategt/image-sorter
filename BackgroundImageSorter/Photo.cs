﻿namespace BackgroundImageSorter
{
    [System.Serializable]
    class Photo
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
    }
}