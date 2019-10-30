using System;
using Xamarin.Forms;

namespace okKindredXamarin
{
    public interface IBaseUrl { string Get(); }

    public class LocalHtmlBaseUrl : ContentPage
    {
        WebView browser;

        public LocalHtmlBaseUrl()
        {
            this.browser = new WebView();
            this.browser.Source = DependencyService.Get<IBaseUrl>().Get() + "index.html";
            Content = this.browser;

            this.browser.Navigating += this.Browser_Navigating;
        }

        protected override bool OnBackButtonPressed()
        {

            if (this.browser.CanGoBack)
            {
                this.browser.GoBack();
                return true;
            }
            else
            {
                base.OnBackButtonPressed();
                return true;
            }
        }

        private void Browser_Navigating(object sender, WebNavigatingEventArgs e)
        {
            if (e.Url.StartsWith("mailto"))
            {
                Device.OpenUri(new Uri(e.Url));
                e.Cancel = true;
            }
        }
    }
}
