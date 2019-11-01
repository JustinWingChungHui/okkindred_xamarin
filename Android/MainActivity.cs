using System;

using Android.App;
using Android.Content;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.OS;

using Xamarin.Forms.Platform.Android;
using Android.Content.PM;



namespace okKindredXamarin.Android
{
	[Activity (Label = "ok!Kindred", Icon = "@drawable/icon", Theme = "@style/MainTheme",  
		ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation)]
    // App link
    [IntentFilter(new[] { Intent.ActionView },
        DataScheme = "https",
        DataHost = "www.okkindred.com",
        AutoVerify = true,
        Categories = new[] { Intent.CategoryDefault, Intent.CategoryBrowsable })]
	public class MainActivity : 
	global::Xamarin.Forms.Platform.Android.FormsAppCompatActivity // superclass new in 1.3
	{
		protected override void OnCreate (Bundle bundle)
		{
			base.OnCreate (bundle);

			global::Xamarin.Forms.Forms.Init (this, bundle);

			LoadApplication (new App ()); // method is new in 1.3
		}

        protected override void OnNewIntent(Intent intent)
        {
            base.OnNewIntent(intent);
        }
    }


}

