﻿using System;

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
	[Activity (Label = "ok!Kindred", Icon = "@drawable/icon", Theme = "@style/MainTheme",  
		ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation)]
    // App link
    [IntentFilter(new[] { Intent.ActionView },
        DataScheme = "https",
        DataHost = "www.okkindred.com",
        AutoVerify = true,
        Categories = new[] { Intent.CategoryDefault, Intent.CategoryBrowsable })]
    [IntentFilter(new[] { Intent.ActionSend }, 
        Categories = new[] { Intent.CategoryDefault }, DataMimeType = "image/*", Label = "ok!Kindred")]
    [IntentFilter(new string[] { Intent.ActionSendMultiple },
        Categories = new string[] { Intent.CategoryDefault }, DataMimeType = "image/*", Label = "ok!Kindred")]
    public class MainActivity : 
	global::Xamarin.Forms.Platform.Android.FormsAppCompatActivity // superclass new in 1.3
	{
		protected override void OnCreate (Bundle bundle)
		{
            this.CheckAppPermissions();

            base.OnCreate (bundle);

			global::Xamarin.Forms.Forms.Init (this, bundle);

            var app = new App();


            LoadApplication(app); // method is new in 1.3

            // Handle shared image
            if (Intent.Action == Intent.ActionSend && Intent.Extras.ContainsKey(Intent.ExtraStream))
            {
                var uri = (Android.Net.Uri)Intent.Extras.GetParcelable(Intent.ExtraStream);
                // var fileUrl = GetFilePath(uri);

                var fileUrl = FilePathResolver.GetFilePath(ApplicationContext, uri);
                var image = new UploadImage(fileUrl, Intent.Type);
                var images = new List<UploadImage> { image };
                app.setImageToUpload(images);
            } 
            
            // Handle multiple shared images
            else if (Intent.Action == Intent.ActionSendMultiple)
            {
                var images = new List<UploadImage>();

                var fileObjects = Intent.Extras.GetParcelableArrayList(Intent.ExtraStream);
                foreach (var obj in fileObjects)
                {
                    // var fileUrl = GetFilePath((Android.Net.Uri)obj);
                    var fileUrl = FilePathResolver.GetFilePath(ApplicationContext, (Android.Net.Uri)obj);
                    var image = new UploadImage(fileUrl, Intent.Type);

                    images.Add(image);
                }

                app.setImageToUpload(images);
            }
        }

        protected override void OnNewIntent(Intent intent)
        {
            base.OnNewIntent(intent);
        }

        private string GetFilePath(Android.Net.Uri uri)
        {
            string[] proj = { MediaStore.Images.ImageColumns.Data };
            var cursor = ContentResolver.Query(uri, proj, null, null, null);
            var colIndex = cursor.GetColumnIndex(MediaStore.Images.ImageColumns.Data);
            cursor.MoveToFirst();
            return cursor.GetString(colIndex);
        }


        private void CheckAppPermissions()
        {
            if ((int)Build.VERSION.SdkInt < 23)
            {
                return;
            }
            else
            {
                if (PackageManager.CheckPermission(Manifest.Permission.ReadExternalStorage, PackageName) != Permission.Granted
                    && PackageManager.CheckPermission(Manifest.Permission.WriteExternalStorage, PackageName) != Permission.Granted)
                {
                    var permissions = new string[] { Manifest.Permission.ReadExternalStorage, Manifest.Permission.WriteExternalStorage };
                    RequestPermissions(permissions, 1);
                }
            }
        }
    }


}

