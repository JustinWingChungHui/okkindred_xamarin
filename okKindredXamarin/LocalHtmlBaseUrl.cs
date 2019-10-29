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
    }
}
