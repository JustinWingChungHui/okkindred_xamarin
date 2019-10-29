using System;
using Xamarin.Forms;
using okKindredXamarin;
using okKindredXamarin.Android;

[assembly: Dependency (typeof (BaseUrl_Android))]
namespace okKindredXamarin.Android 
{
	public class BaseUrl_Android : IBaseUrl 
	{
		public string Get () 
		{
            return "file:///android_asset/";
        }
	}
}