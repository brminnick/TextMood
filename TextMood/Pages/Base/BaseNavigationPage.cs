using Xamarin.Forms.PlatformConfiguration;
using Xamarin.Forms.PlatformConfiguration.iOSSpecific;

namespace TextMood
{
    public class BaseNavigationPage : Xamarin.Forms.NavigationPage
    {
        public BaseNavigationPage(Xamarin.Forms.Page root) : base(root)
        {
            BarTextColor = ColorConstants.BarTextColor;
            BarBackgroundColor = ColorConstants.BarBackgroundColor;

            On<iOS>().SetPrefersLargeTitles(true);
        }
    }
}
