using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using okKindredXamarin.Models;
using Plugin.Media;
using Plugin.Media.Abstractions;
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
                this.browser.GoBack();
                return true;
            }

            else
            {
                return base.OnBackButtonPressed();
            }
        }

        private async void Browser_Navigating(object sender, WebNavigatingEventArgs e)
        {
            try
            {
                var prefixes = new List<string> { "mailto", "http", "www" };

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
            catch(Exception ex)
            {
                await Application.Current.MainPage.DisplayAlert("Error", ex.Message, "Ok");
            }
        }

        private async Task UploadFileFromFilePicker(bool multiSelect)
        {
            await CrossMedia.Current.Initialize();

            var files = new List<MediaFile>() ;

            if (multiSelect)
            {
                files = await CrossMedia.Current.PickPhotosAsync(
                    new PickMediaOptions
                    {
                        RotateImage = false,
                    }, 
                    new MultiPickerOptions
                    {
                        MaximumImagesCount = 10,
                    });
            }
            else
            {
                var file = await CrossMedia.Current.PickPhotoAsync(new PickMediaOptions
                {
                    RotateImage = false
                });

                if (file != null)
                {
                    files = new List<MediaFile> { file };
                }
            }

            if (files != null && files.Any()) { 
                var uploadImages = new List<UploadImage>();

                foreach (var file in files)
                {
                    using (var stream = file.GetStream())
                    {
                        uploadImages.Add(new UploadImage(stream, file.Path));
                    }
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
