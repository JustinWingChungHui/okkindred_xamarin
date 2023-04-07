using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xamarin.Essentials;

namespace okKindredXamarin.Models
{
    public class MediaImageResolver
    {
        private readonly List<FileResult> _xamarinMediaFiles;

        public MediaImageResolver(IEnumerable<FileResult> xamarinMediaFiles)
        {
            this._xamarinMediaFiles = xamarinMediaFiles.ToList();
        }

        public UploadImages GetImageDetails()
        {
            var result = new UploadImages(UploadImages.ImageSource.FilePicker);
            

            for (var i = 0; i < this._xamarinMediaFiles.Count; i++)
            {
                result.Add(new UploadImage(i, this._xamarinMediaFiles[i].FullPath));
            }

            return result;
        }


        public async Task<UploadImage> GetImageData(int index)
        {            
            if (this._xamarinMediaFiles != null && this._xamarinMediaFiles.Count > index)
            {
                var mediaFile = this._xamarinMediaFiles[index];
                using (var stream = await mediaFile.OpenReadAsync())
                {
                    return new UploadImage(index, stream, mediaFile.FullPath);
                }

            }
            return null;
        }
    }
}
