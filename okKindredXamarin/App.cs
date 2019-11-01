using System;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace okKindredXamarin
{
	public class App : Application
	{
        public Uri AppLink { get; set; }

		public App ()
		{
			this.MainPage = new LocalHtmlBaseUrl { Title = "BaseUrl" };
		}

        protected override void OnAppLinkRequestReceived(Uri uri)
        {
            if (uri.Host.EndsWith("okkindred.com", StringComparison.OrdinalIgnoreCase))
            {
                this.AppLink = uri;
                var page = this.MainPage as LocalHtmlBaseUrl;
                // Make sure browser has navigated first.
                page.browser.Navigated += Browser_Navigated;

            }
        }

        private void Browser_Navigated(object sender, WebNavigatedEventArgs e)
        {
            var page = this.MainPage as LocalHtmlBaseUrl;
            page.browser.Navigated -= Browser_Navigated;

            var url = this.AppLink.ToString();

            if (url.Contains("#"))
            {
                var route = url.Substring(url.IndexOf("#") + 1).Trim();
                Device.BeginInvokeOnMainThread(async () =>
                {
                    // Use Vue router to navigate to link
                    await page.browser.EvaluateJavaScriptAsync($"viewModel.navigateTo('{route}');");
                });
            }
        }
    }
}

