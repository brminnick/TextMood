using Foundation;
using Microsoft.Extensions.DependencyInjection;
using UIKit;

namespace TextMood.iOS
{
	[Register(nameof(AppDelegate))]
    public partial class AppDelegate : global::Xamarin.Forms.Platform.iOS.FormsApplicationDelegate
    {
        public override bool FinishedLaunching(UIApplication uiApplication, NSDictionary launchOptions)
        {
            global::Xamarin.Forms.Forms.Init();

            var app = ServiceCollection.ServiceProvider.GetRequiredService<App>();
            LoadApplication(app);

            return base.FinishedLaunching(uiApplication, launchOptions);
        }
    }
}
