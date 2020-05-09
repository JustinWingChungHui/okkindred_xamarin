using Plugin.Media.Abstractions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace okKindredXamarin.Models
{
    public class UploadImages
    {
        private readonly List<MediaFile> _mediaFiles;

        public UploadImages(List<MediaFile> mediaFiles)
        {
            this._mediaFiles = mediaFiles;
        }

        public List<UploadImage> GetImageDetails()
        {
            var result = new List<UploadImage>();
            
            for (var i = 0; i < this._mediaFiles.Count; i++)
            {
                result.Add(new UploadImage(i, this._mediaFiles[i].Path));
            }

            return result;
        }


        public UploadImage GetImageData(int index)
        {            
            if (this._mediaFiles != null && this._mediaFiles.Count > index)
            {
                var mediaFile = this._mediaFiles[index];
                using (var stream = mediaFile.GetStream())
                {
                    return new UploadImage(index, stream, mediaFile.Path);
                }

            }
            return null;
        }
    }
}
