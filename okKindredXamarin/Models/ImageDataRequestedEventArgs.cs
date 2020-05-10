using System;
using System.Collections.Generic;
using System.Text;

namespace okKindredXamarin.Models
{
    public class ImageDataRequestedEventArgs : EventArgs
    {
        public ImageDataRequestedEventArgs(int index, UploadImages.ImageSource source)
        {
            this.Index = index;
            this.Source = source;
        }

        public int Index { get; }
        public UploadImages.ImageSource Source { get; }
    }
}
