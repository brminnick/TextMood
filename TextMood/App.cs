using Xamarin.Forms;
using Xamarin.Forms.PlatformConfiguration;
using Xamarin.Forms.PlatformConfiguration.iOSSpecific;

namespace TextMood
{
    public class App : Xamarin.Forms.Application
    {
        public App()
        {
            var navigationPage = new BaseNavigationPage(new TextResultsListPage());
            navigationPage.On<iOS>().SetPrefersLargeTitles(true);

            MainPage = navigationPage;
        }
    }
}
