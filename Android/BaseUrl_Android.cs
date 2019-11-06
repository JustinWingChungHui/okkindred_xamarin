using System;
using Xamarin.Forms;
using okKindredXamarin;
using okKindredXamarin.Droid;

[assembly: Dependency (typeof (BaseUrl_Android))]
namespace okKindredXamarin.Droid 
{
	public class BaseUrl_Android : IBaseUrl 
	{
		public string Get () 
		{
            return "file:///android_asset/";
        }
	}
}