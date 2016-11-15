using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SearchDemo.ViewModels.Site
{
    public class FileViewModel
    {
        public int ID { get; set; }
        public string Name { get; set; }
        public string ContentType { get; set; }
        public string Link { get; set; }
        public int FolderID { get; set; }
        public DateTime CreateDateTime { get; set; }
        public string Size { get; set; }
        public string Dimension { get; set; }
        public string Resolution { get; set; }

        public string ToThumbnailLink()
        {
            Char delimiter = '.';
            string[] name = Name.Split(delimiter);
            string thumbNailLink = Link.Remove(Link.Length - Name.Length, Name.Length) + name.First() + "-thumbnail" + "." + name.Last();
            return thumbNailLink;
        }
        public string ToFileSize(int fileSize)
        {
            string[] sizes = { "B", "KB", "MB", "GB" };
            double len = fileSize;
            string result = "";
            int order = 0;
            while (len >= 1024 && order + 1 < sizes.Length)
            {
                order++;
                len = len / 1024;
            }

            // Adjust the format string to your preferences. For example "{0:0.#}{1}" would
            // show a single decimal place, and no space.
            result = $"{len:0.##} {sizes[order]}";

            return result;
        }
    }
}
