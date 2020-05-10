using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace okKindredXamarin.Models
{
    public class UploadImages : List<UploadImage>
    {
        public enum ImageSource
        {
            FilePicker,
            AndroidShare
        }

        public UploadImages(ImageSource source)
        {
            this.Source = source;
        }

        public ImageSource Source { get; }

        public override string ToString()
        {
            return $"[{string.Join(",", this.Select(i => i.ToString()))}]";
        }
    }
}
