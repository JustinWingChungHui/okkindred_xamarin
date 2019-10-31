using System;
using Xamarin.Forms;

namespace okKindredXamarin
{
	public class App : Application
	{
		public App ()
		{
			this.MainPage = new LocalHtmlBaseUrl { Title = "BaseUrl" };
		}

        protected override void OnAppLinkRequestReceived(Uri uri)
        {
            if (uri.Host.EndsWith("okkindred.com", StringComparison.OrdinalIgnoreCase))
            {
                var page = this.MainPage as LocalHtmlBaseUrl;
                var route = uri.ToString().Substring(uri.ToString().IndexOf("#") + 1).Trim();
                page.browser.Source = DependencyService.Get<IBaseUrl>().Get() + "index.html/#/" + route;
            }
        }
    }
}

