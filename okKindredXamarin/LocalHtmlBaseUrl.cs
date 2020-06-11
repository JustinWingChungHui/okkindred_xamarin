using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
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
        private MediaImageResolver _mediaPickerImageResolver;

        public UploadImages UploadImages { get; set; }

        public WebView browser;

        public event EventHandler<ImageDataRequestedEventArgs> ImageDataRequested;
        

        public LocalHtmlBaseUrl()
        {
            this.browser = new WebView
            {
                Source = DependencyService.Get<IBaseUrl>().Get() + "index.html"
            };

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

                if (e.Url.Contains("xamarin_request_android_image_data"))
                {
                    e.Cancel = true;
                    // Match number at the end
                    var regex = new Regex(@"\d+$");
                    var index = regex.Match(e.Url).Value;
                    this.UploadFileData(int.Parse(index));
                }
            }
            catch(Exception ex)
            {
                await Application.Current.MainPage.DisplayAlert("Error", ex.Message, "Ok");
            }
        }

        private void UploadFileData(int index)
        {
            if (this.UploadImages != null)
            {
                if (this.UploadImages.Source == UploadImages.ImageSource.FilePicker)
                {
                    Device.BeginInvokeOnMainThread(async () =>
                    {

                        var imageParam = _mediaPickerImageResolver.GetImageData(index).ToString();
                        var cmd = $"viewModel.uploadAndroidImageData({imageParam});";
                        await this.browser.EvaluateJavaScriptAsync(cmd);
                    });
                }
                else if (this.UploadImages.Source == UploadImages.ImageSource.AndroidShare)
                {
                    this.ImageDataRequested?.Invoke(this, new ImageDataRequestedEventArgs(index, UploadImages.ImageSource.AndroidShare));
                }
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
                        MaximumImagesCount = 100,
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

                this._mediaPickerImageResolver = new MediaImageResolver(files);
                this.UploadImages = _mediaPickerImageResolver.GetImageDetails();

                if (!multiSelect)
                {
                    this.UploadImages[0].Data = _mediaPickerImageResolver.GetImageData(0).Data;
                }

                Device.BeginInvokeOnMainThread(async () =>
                {
                    var cmd = $"viewModel.uploadAndroidImageDetails({this.UploadImages});";
                    await this.browser.EvaluateJavaScriptAsync(cmd);
                });

            }
        }
    }
}
