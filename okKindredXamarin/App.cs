using okKindredXamarin.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace okKindredXamarin
{
	public class App : Application
	{
        private List<UploadImage> _uploadImages;

        private Uri _appLink { get; set; }

        private WebViewAction _action;

        public WebView Browser
        {
            get
            {
                return (this.MainPage as LocalHtmlBaseUrl).browser;
            }
        }

        // Defines list of actions that can happen once webview is loaded
        public enum WebViewAction
        {
            none,
            route,
            uploadImage,
        }
                               

        public App ()
		{
            this._uploadImages = null;
            this._action = WebViewAction.none;
            this.MainPage = new LocalHtmlBaseUrl { Title = "BaseUrl" };
		}

        public void SetSharedImagesToUpload(List<UploadImage> uploadImage)
        {
            this._uploadImages = uploadImage;
            this._action = WebViewAction.uploadImage;

            // Make sure browser has navigated first.
            this.Browser.Navigated += Browser_Navigated;
        }

        protected override void OnAppLinkRequestReceived(Uri uri)
        {
            if (uri.Host.EndsWith("okkindred.com", StringComparison.OrdinalIgnoreCase))
            {
                this._appLink = uri;
                this._action = WebViewAction.route;

                // Make sure browser has navigated first.
                this.Browser.Navigated += Browser_Navigated;
            }
        }

        private void Browser_Navigated(object sender, WebNavigatedEventArgs e)
        {
            this.Browser.Navigated -= Browser_Navigated;

            switch(this._action)
            {
                case WebViewAction.none:
                    break;

                case WebViewAction.route:
                    this.NavigateToRoute();
                    break;

                case WebViewAction.uploadImage:
                    this.UploadImage();
                    break;
            }

           
        }

        private void UploadImage()
        {
            if (this._uploadImages != null && this._uploadImages.Count > 0)
            {
                Device.BeginInvokeOnMainThread(async () =>
                {

                    var imageParams = $"[{string.Join(",", this._uploadImages.Select(i => i.ToString()))}]";
                    var cmd = $"viewModel.uploadFiles({imageParams});";
                    await this.Browser.EvaluateJavaScriptAsync(cmd);
                });
            }
        }

        private void NavigateToRoute()
        {
            if (this._appLink != null)
            {
                var url = this._appLink.ToString();

                if (url.Contains("#"))
                {
                    var route = url.Substring(url.IndexOf("#") + 1).Trim();
                    Device.BeginInvokeOnMainThread(async () =>
                    {
                        // Use Vue router to navigate to link
                        await this.Browser.EvaluateJavaScriptAsync($"viewModel.navigateTo('{route}');");
                    });
                }
            }
        }
    }
}

