using Android.App;
using Android.Content;
using Android.OS;
using Android.Content.PM;
using Android;
using okKindredXamarin.Models;
using System.Collections.Generic;
using System.Collections;
using Android.Views;
using Xamarin.Forms;

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
        private App _app;

        private IList _sharedIntentImages;

        protected override void OnCreate (Bundle bundle)
		{
            Window.SetSoftInputMode(Android.Views.SoftInput.AdjustResize);

            this.CheckAppPermissions();

            base.OnCreate(bundle);

            global::Xamarin.Forms.Forms.Init (this, bundle);
            Xamarin.Essentials.Platform.Init(this, bundle);

            this._app = new App();
            LoadApplication(this._app); // method is new in 1.3

            // Handle shared image
            if (Intent.Action == Intent.ActionSend && Intent.Extras.ContainsKey(Intent.ExtraStream))
            {
                var uri = (Android.Net.Uri)Intent.Extras.GetParcelable(Intent.ExtraStream);
                this._sharedIntentImages = new List<Android.Net.Uri> { uri };
                var images = SharedContentResolver.CreateUploadImagesNoData(ApplicationContext, this._sharedIntentImages);
                this._app.SetSharedImageDetailsToUpload(images);
                this._app.ImageDataRequested += App_SharedImageDataRequested;
            } 
            
            // Handle multiple shared images
            else if (Intent.Action == Intent.ActionSendMultiple) 
            {                
                this._sharedIntentImages = Intent.Extras.GetParcelableArrayList(Intent.ExtraStream);
                var images = SharedContentResolver.CreateUploadImagesNoData(ApplicationContext, this._sharedIntentImages);
                this._app.SetSharedImageDetailsToUpload(images);
                this._app.ImageDataRequested += App_SharedImageDataRequested;
            }

            this._app.BrowserNavigating += app_BrowserNavigating;
            this._app.BrowserNavigated += app_BrowserNavigated;
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
                    || PackageManager.CheckPermission(Manifest.Permission.WriteExternalStorage, PackageName) != Permission.Granted
                    || PackageManager.CheckPermission(Manifest.Permission.AccessMediaLocation, PackageName) != Permission.Granted)
                {
                    var permissions = new string[] 
                    { 
                        Manifest.Permission.ReadExternalStorage, 
                        Manifest.Permission.WriteExternalStorage, 
                        Manifest.Permission.AccessMediaLocation 
                    };
                    RequestPermissions(permissions, 1);
                }
            }
        }

        // https://github.com/jamesmontemagno/MediaPlugin
        public override void OnRequestPermissionsResult(int requestCode, string[] permissions, Android.Content.PM.Permission[] grantResults)
        {
            Xamarin.Essentials.Platform.OnRequestPermissionsResult(requestCode, permissions, grantResults);
            base.OnRequestPermissionsResult(requestCode, permissions, grantResults);
        }

        private void App_SharedImageDataRequested(object sender, ImageDataRequestedEventArgs e)
        {
            if (this._sharedIntentImages != null && e.Source == UploadImages.ImageSource.AndroidShare)
            {
                var imageWithData = SharedContentResolver.CreateUploadImageWithData(e.Index, ApplicationContext, this._sharedIntentImages);
                this._app.SetSharedImageDataToUpload(imageWithData);
            }
        }

        private void app_BrowserNavigating(object sender, Xamarin.Forms.WebNavigatingEventArgs e)
        {
            // Keep screen on when uploading images
            if (e.Url.Contains("xamarin_request_android_image_data"))
            {
                Window.AddFlags(WindowManagerFlags.KeepScreenOn);
            }
        }

        private void app_BrowserNavigated(object sender, WebNavigatedEventArgs e)
        {
            Window.ClearFlags(WindowManagerFlags.KeepScreenOn);
        }
    }


}

