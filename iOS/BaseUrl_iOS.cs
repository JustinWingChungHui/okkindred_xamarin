using Xamarin.Forms;
using WorkingWithWebview.iOS;
using Foundation;
using okKindredXamarin;

[assembly: Dependency (typeof (BaseUrl_iOS))]

namespace WorkingWithWebview.iOS 
{
	public class BaseUrl_iOS : IBaseUrl 
	{
		public string Get () 
		{
			return NSBundle.MainBundle.BundlePath;
		}
	}
}