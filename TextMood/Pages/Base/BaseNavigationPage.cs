using Xamarin.Forms;

namespace TextMood
{
	public class BaseNavigationPage : NavigationPage
	{
		public BaseNavigationPage(Page root) : base(root)
		{
			BarTextColor = ColorConstants.BarTextColor;
			BarBackgroundColor = ColorConstants.BarBackgroundColor;
		}
	}
}
