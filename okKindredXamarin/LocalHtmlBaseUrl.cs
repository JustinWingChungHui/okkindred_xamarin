﻿using System;
using System.Collections.Generic;
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

        private void Browser_Navigating(object sender, WebNavigatingEventArgs e)
        {

            var prefixes = new List<string>{ "mailto", "http", "www" };

            foreach (var prefix in prefixes)
            {
                if (e.Url.ToLowerInvariant().StartsWith(prefix))
                {
                    Device.OpenUri(new Uri(e.Url));
                    e.Cancel = true;

                    return;
                }
            }
        }
    }
}
