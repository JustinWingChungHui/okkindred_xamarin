using okKindredXamarin.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using Xamarin.Forms;
using AndroidSpecific = Xamarin.Forms.PlatformConfiguration.AndroidSpecific;

namespace okKindredXamarin
{
	public class App : Application
	{
        private readonly Queue<WebViewAction> _actions;

        private List<UploadImage> _uploadImages;

        private Uri _appLink;

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
            startup,
            route,
            shareImage,
        }
                               

        public App ()
		{
            // https://stackoverflow.com/questions/48425623/resize-webview-when-i-show-the-keyboard
            AndroidSpecific.Application.SetWindowSoftInputModeAdjust(this, AndroidSpecific.WindowSoftInputModeAdjust.Resize);
            this._uploadImages = null;
            this._actions = new Queue<WebViewAction>();
            this._actions.Enqueue(WebViewAction.startup);
            this.MainPage = new LocalHtmlBaseUrl { Title = "BaseUrl" };

            this.Browser.Navigated += Browser_Navigated;
        }


        public void SetSharedImagesToUpload(List<UploadImage> uploadImage)
        {
            this._uploadImages = uploadImage;
            this._actions.Enqueue(WebViewAction.shareImage);

            // Make sure browser has navigated first.
            this.Browser.Navigated += Browser_Navigated;
        }

        protected override void OnAppLinkRequestReceived(Uri uri)
        {
            if (uri.Host.EndsWith("okkindred.com", StringComparison.OrdinalIgnoreCase))
            {
                this._appLink = uri;
                this._actions.Enqueue(WebViewAction.route);

                // Make sure browser has navigated first.
                this.Browser.Navigated += Browser_Navigated;
            }
        }

        private void Browser_Navigated(object sender, WebNavigatedEventArgs e)
        {
            this.Browser.Navigated -= Browser_Navigated;

            while (this._actions.Count > 0)
            {
                var action = this._actions.Dequeue();

                switch (action)
                {
                    case WebViewAction.startup:
                        this.OnStartup();
                        break;

                    case WebViewAction.route:
                        this.NavigateToRoute();
                        break;

                    case WebViewAction.shareImage:
                        this.ShareImages();
                        break;
                }
            }
        }

        private void OnStartup()
        {
            Device.BeginInvokeOnMainThread(async () =>
            {
                var userAgent = await this.Browser.EvaluateJavaScriptAsync("navigator.userAgent");
                var cmd = $"viewModel.setUserAgent('{userAgent} xamarin');";
                await this.Browser.EvaluateJavaScriptAsync(cmd);
            });
        }

        private void ShareImages()
        {
            if (this._uploadImages != null && this._uploadImages.Count > 0)
            {
                Device.BeginInvokeOnMainThread(async () =>
                {

                    var imageParams = $"[{string.Join(",", this._uploadImages.Select(i => i.ToString()))}]";
                    var cmd = $"viewModel.uploadAndroidSharedFiles({imageParams});";
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

