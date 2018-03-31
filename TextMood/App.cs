using Xamarin.Forms;

namespace TextMood
{
	public class App : Application
    {
		public App()
		{
			MainPage = new BaseNavigationPage(new TextResultsListPage());
		}

		protected override async void OnStart()
		{
			base.OnStart();

			await SignalRService.Subscribe().ConfigureAwait(false);
		}
	}
}
