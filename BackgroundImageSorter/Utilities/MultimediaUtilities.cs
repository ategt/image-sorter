using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BackgroundImageSorter.Utilities
{
    public class MultimediaUtilities
    {
        public static string DetectProperExtension(string mediaString, MediaToolkit.Model.Metadata metadata = null)
        {
            metadata = metadata ?? getMetadata(mediaString);
            string format = metadata.VideoData?.Format;

            if (string.IsNullOrWhiteSpace(format))
                format = metadata.AudioData?.Format;

            if (format.Contains(" "))
                format = format.Split(new string[] { " " }, StringSplitOptions.RemoveEmptyEntries)[0];

            return format;
        }

        private static MediaToolkit.Model.Metadata getMetadata(string mediaString)
        {
            MediaToolkit.Model.MediaFile mediaFile = new MediaToolkit.Model.MediaFile(mediaString);

            using (MediaToolkit.Engine engine = new MediaToolkit.Engine())
            {
                engine.GetMetadata(mediaFile);
            }

            MediaToolkit.Model.Metadata metadata = mediaFile.Metadata;
            return metadata;
        }

        public static bool IsVideo(Uri uri, MediaToolkit.Model.Metadata metadata = null)
        {
            metadata = metadata ?? getMetadata(uri.LocalPath);
            return metadata.VideoData != null;
        }

        public static bool IsAudio(Uri uri, MediaToolkit.Model.Metadata metadata = null)
        {
            metadata = metadata ?? getMetadata(uri.LocalPath);
            return metadata.AudioData != null;
        }
        public static bool IsMultimedia(Uri uri, MediaToolkit.Model.Metadata metadata = null)
        {
            metadata = metadata ?? getMetadata(uri.LocalPath);
            return metadata != null && (metadata.AudioData != null || metadata.VideoData != null);
        }
    }
}
