using okKindredXamarin.UWP;
using Xamarin.Forms;

[assembly: Dependency(typeof(BaseUrl))]
namespace okKindredXamarin.UWP
{
    public class BaseUrl : IBaseUrl
    {
        public string Get()
        {
            return "ms-appx-web:///WebAssets/";
        }
    }
}
