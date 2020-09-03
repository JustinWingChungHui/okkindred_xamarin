using Android.Content;
using Android.Provider;
using System.IO;
using okKindredXamarin.Models;
using System.Collections;

namespace okKindredXamarin.Droid
{
    public class SharedContentResolver
    {
        public static UploadImages CreateUploadImagesNoData(Context context, IList parcelableContent)
        {
            var result = new UploadImages(UploadImages.ImageSource.AndroidShare);
            int index = 0;
            foreach (var obj in parcelableContent)
            {
                var image = CreateUploadImageNoData(index, context, (Android.Net.Uri)obj);
                result.Add(image);
                index++;
            }

            return result;
        }

        /// <summary>
        /// Creates the Upload Image object to pass to the webview 
        /// from an Android Uri
        /// </summary>
        public static UploadImage CreateUploadImageWithData(int index, Context context, IList parcelableContent)
        {
            if (parcelableContent.Count <= index) {
                throw new OperationApplicationException($"index:{index} not avaibale in shared images");
            }

            var uri = (Android.Net.Uri)parcelableContent[index];

            byte[] data;
            using (var stream = context.ContentResolver.OpenInputStream(uri))
            {
                using (var ms = new MemoryStream())
                {
                    stream.CopyTo(ms);
                    data = ms.ToArray();
                }
            }

            var fileName = GetFileName(context, uri);
            var image = new UploadImage(index, data, fileName);

            return image;
            
        }

        /// <summary>
        /// Creates the Upload Image object to pass to the webview 
        /// from an Android Uri
        /// </summary>
        public static UploadImage CreateUploadImageNoData(int index, Context context, Android.Net.Uri uri)
        {
            var fileName = GetFileName(context, uri);
            var image = new UploadImage(index, fileName);

            return image;
        }

        /// <summary>
        /// Gets the filename from an Android Uri
        /// </summary>
        public static string GetFileName(Context context, Android.Net.Uri uri)
        {
            string filename = "";
            string[] projection = { MediaStore.IMediaColumns.DisplayName };
            var metaCursor = context.ContentResolver.Query(uri, projection, null, null, null);
            if (metaCursor != null)
            {
                try
                {
                    if (metaCursor.MoveToFirst())
                    {
                        filename = metaCursor.GetString(0);
                    }
                }
                finally
                {
                    metaCursor.Close();
                }
            }

            return filename;
        }
    }
}

