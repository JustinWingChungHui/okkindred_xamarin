using Android.Content;
using Android.Provider;
using System.IO;
using okKindredXamarin.Models;


namespace okKindredXamarin.Droid
{
    public class SharedContentResolver
    {
        /// <summary>
        /// Creates the Upload Image object to pass to the webview 
        /// from an Android Uri
        /// </summary>
        public static UploadImage CreateUploadImage(Context context, Android.Net.Uri uri, string type)
        {
            byte[] data;
            using (var stream = context.ContentResolver.OpenInputStream(uri))
            {
                using MemoryStream ms = new MemoryStream();
                stream.CopyTo(ms);
                data = ms.ToArray();
            }

            var fileName = GetFileName(context, uri);
            var image = new UploadImage(0, data, fileName, type);

            return image;
        }

        /// <summary>
        /// Gets the filename from an Android Uri
        /// </summary>
        public static string GetFileName(Context context, Android.Net.Uri uri)
        {
            string filename = "";
            string[] projection = { MediaStore.MediaColumns.DisplayName };
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

