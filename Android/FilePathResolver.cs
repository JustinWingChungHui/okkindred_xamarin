using System;

using Android.App;
using Android.Content;
using Android.Net;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.OS;

using Xamarin.Forms.Platform.Android;
using Android.Content.PM;
using Android.Provider;
using System.IO;
using Android;
using System.Buffers.Text;
using okKindredXamarin.Models;
using System.Collections.Generic;

namespace okKindredXamarin.Droid
{
    // Oh my God...
    // https://github.com/iPaulPro/aFileChooser/blob/master/aFileChooser/src/com/ipaulpro/afilechooser/utils/FileUtils.java





    /**
     * Get a file path from a Uri. This will get the the path for Storage Access
     * Framework Documents, as well as the _data field for the MediaStore and
     * other file-based ContentProviders.
     *
     * @param context The context.
     * @param uri The Uri to query.
     */
    public class FilePathResolver
    {
        public static string GetFilePath(Context context, Android.Net.Uri uri)
        {
            if (DocumentsContract.IsDocumentUri(context, uri))
            {

               if (isExternalStorageDocument(uri))
                {
                    var docId = DocumentsContract.GetDocumentId(uri);
                    var split = docId.Split(":");
                    var type = split[0];

                    if ("primary".Equals(type, StringComparison.OrdinalIgnoreCase))
                    {
                        return Android.OS.Environment.ExternalStorageDirectory + "/" + split[1];
                    }

                    // TODO handle non-primary volumes
                }
                // DownloadsProvider
                else if (isDownloadsDocument(uri))
                {

                    var id = DocumentsContract.GetDocumentId(uri);
                    var contentUri = ContentUris.WithAppendedId(
                           Android.Net.Uri.Parse("content://downloads/public_downloads"), long.Parse(id));

                    return GetDataColumn(context, uri, null, null);
                }
                // MediaProvider
                else if (isMediaDocument(uri))
                {
                    var docId = DocumentsContract.GetDocumentId(uri);
                    var split = docId.Split(":");
                    var type = split[0];

                    Android.Net.Uri contentUri = null;
                    if ("image".Equals(type, StringComparison.OrdinalIgnoreCase))
                    {
                        contentUri = MediaStore.Images.Media.ExternalContentUri;
                    }
                    else if ("video".Equals(type, StringComparison.OrdinalIgnoreCase))
                    {
                        contentUri = MediaStore.Video.Media.ExternalContentUri;
                    }
                    else if ("audio".Equals(type, StringComparison.OrdinalIgnoreCase))
                    {
                        contentUri = MediaStore.Audio.Media.ExternalContentUri;
                    }

                    var selection = "_id=?";
                    var selectionArgs = new string[] {
                    split[1]
                };

                    return GetDataColumn(context, contentUri, selection, selectionArgs);
                }

            }

            // MediaStore (and general)
            else if ("content".Equals(uri.Scheme, StringComparison.OrdinalIgnoreCase))
            {

                // Return the remote address
                if (isGooglePhotosUri(uri))
                    return uri.LastPathSegment;

                return GetDataColumn(context, uri, null, null);
            }
            // File
            else if ("file".Equals(uri.Scheme, StringComparison.OrdinalIgnoreCase))
            {
                return uri.Path;
            }

            return null;
        }

        /**
         * Get the value of the data column for this Uri. This is useful for
         * MediaStore Uris, and other file-based ContentProviders.
         *
         * @param context The context.
         * @param uri The Uri to query.
         * @param selection (Optional) Filter used in the query.
         * @param selectionArgs (Optional) Selection arguments used in the query.
         * @return The value of the _data column, which is typically a file path.
         */
        public static string GetDataColumn(Context context, Android.Net.Uri uri, string selection, string[] selectionArgs)
        {
            Android.Database.ICursor cursor = null;
            var column = "_data";
            string[] projection = {
                column
            };

            try
            {
                cursor = context.ContentResolver.Query(uri, projection, selection, selectionArgs,  null);
                if (cursor != null && cursor.MoveToFirst())
                {
                    var column_index = cursor.GetColumnIndexOrThrow(column);
                    return cursor.GetString(column_index);
                }
            }
            finally
            {
                if (cursor != null)
                {
                    cursor.Close();
                }
            }
            return null;
        }


        public static bool isExternalStorageDocument(Android.Net.Uri uri)
        {
            return "com.android.externalstorage.documents".Equals(uri.Authority, StringComparison.OrdinalIgnoreCase);
        }

        /**
         * @param uri The Uri to check.
         * @return Whether the Uri authority is DownloadsProvider.
         */
        public static bool isDownloadsDocument(Android.Net.Uri uri)
        {
            return "com.android.providers.downloads.documents".Equals(uri.Authority, StringComparison.OrdinalIgnoreCase);
        }

        /**
         * @param uri The Uri to check.
         * @return Whether the Uri authority is MediaProvider.
         */
        public static bool isMediaDocument(Android.Net.Uri uri)
        {
            return "com.android.providers.media.documents".Equals(uri.Authority, StringComparison.OrdinalIgnoreCase);
        }

        public static bool isGooglePhotosUri(Android.Net.Uri uri)
        {
            return "com.google.android.apps.photos.content".Equals(uri.Authority, StringComparison.OrdinalIgnoreCase);
        }
    }
}

