using Plugin.Media.Abstractions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace okKindredXamarin.Models
{
    public class MediaImageResolver
    {
        private readonly List<MediaFile> _mediaFiles;

        public MediaImageResolver(List<MediaFile> mediaFiles)
        {
            this._mediaFiles = mediaFiles;
        }

        public UploadImages GetImageDetails()
        {
            var result = new UploadImages(UploadImages.ImageSource.FilePicker);
            
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
