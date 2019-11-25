using Android.App;
using Android.Content;
using Android.OS;
using Android.Content.PM;
using Android;
using okKindredXamarin.Models;
using System.Collections.Generic;

namespace okKindredXamarin.Droid
{
    [Activity(Theme = "@style/MainTheme.Splash", MainLauncher = true, 
        ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation)]
    // App link
    [IntentFilter(new[] { Intent.ActionView },
        DataScheme = "https",
        DataHost = "www.okkindred.com",
        AutoVerify = true,
        Categories = new[] { Intent.CategoryDefault, Intent.CategoryBrowsable })]
    // Share images
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

                var image = SharedContentResolver.CreateUploadImage(ApplicationContext, uri, Intent.Type);
                var images = new List<UploadImage> { image };
                app.SetSharedImagesToUpload(images);
            } 
            
            // Handle multiple shared images
            else if (Intent.Action == Intent.ActionSendMultiple)
            {
                var images = new List<UploadImage>();

                var fileObjects = Intent.Extras.GetParcelableArrayList(Intent.ExtraStream);
                foreach (var obj in fileObjects)
                {
                    var image = SharedContentResolver.CreateUploadImage(ApplicationContext, (Android.Net.Uri)obj, Intent.Type);
                    images.Add(image);
                }

                app.SetSharedImagesToUpload(images);
            }
        }

        protected override void OnNewIntent(Intent intent)
        {
            base.OnNewIntent(intent);
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

