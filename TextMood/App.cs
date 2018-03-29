using Xamarin.Forms;

namespace TextMood
{
	public class App : Application
    {
		public App()
		{
			MainPage = new NavigationPage(new TextResultsListPage())
			{
				BarTextColor = ColorConstants.BarTextColor,
				BarBackgroundColor = ColorConstants.BarBackgroundColor
			};
		}
    }
}
