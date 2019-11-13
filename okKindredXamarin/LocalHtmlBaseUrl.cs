using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using okKindredXamarin.Models;
using Plugin.Media;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace okKindredXamarin
{
    public interface IBaseUrl { string Get(); }

    public class LocalHtmlBaseUrl : ContentPage
    {
        public WebView browser;

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
                // Starts with double navigation
                this.browser.GoBack();
                if (!this.browser.CanGoBack)
                {
                    return base.OnBackButtonPressed();
                }

                return true;
            }

            else
            {
                return base.OnBackButtonPressed();
            }
        }

        private async void Browser_Navigating(object sender, WebNavigatingEventArgs e)
        {

            var prefixes = new List<string>{ "mailto", "http", "www" };

            foreach (var prefix in prefixes)
            {
                if (e.Url.ToLowerInvariant().StartsWith(prefix))
                {
                    await Launcher.OpenAsync(new Uri(e.Url));
                    e.Cancel = true;

                    return;
                }
            }

            if (e.Url.Contains("xamarin_external_filepicker"))
            {
                e.Cancel = true;
                var uploadMultiple = e.Url.Contains("multiple");
                await this.UploadFileFromFilePicker(uploadMultiple);
            }
        }

        private async Task UploadFileFromFilePicker(bool multiSelect)
        {
            await CrossMedia.Current.Initialize();

            var file = await CrossMedia.Current.PickPhotoAsync(new Plugin.Media.Abstractions.PickMediaOptions
            {
                RotateImage = false
            });

            if (file != null)
            {
                var uploadImages = new List<UploadImage>();
                using (var stream = file.GetStream())
                {
                    uploadImages.Add(new UploadImage(stream, file.Path));
                }

                Device.BeginInvokeOnMainThread(async () =>
                {

                    var imageParams = $"[{string.Join(",", uploadImages.Select(i => i.ToString()))}]";
                    var cmd = $"viewModel.uploadFileFromExternalPicker({imageParams});";
                    await this.browser.EvaluateJavaScriptAsync(cmd);
                });
            }
        }
    }
}
