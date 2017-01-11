namespace BackgroundImageSorter
{
    class Photo
    {
        public int Id { get; set; }
        public byte[] Digest { get; set; }
        public byte[] SHA512 { get; set; }
        public byte[] SHA256 { get; set; }
        public string FileInfo { get; set; }
        public System.Drawing.Size Dimension { get; set; }
        public long Size { get; set; }
        public string Path { get; set; }
    }
}
