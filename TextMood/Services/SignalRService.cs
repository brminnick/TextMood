using System.Threading.Tasks;

using Microsoft.AspNet.SignalR.Client;

using TextMood.Shared;

using Xamarin.Forms;

namespace TextMood
{
	public abstract class SignalRService : BaseSignalRService
	{
		public static bool IsSubscribed => GetIsSubscribed();

        public static async Task Subscribe()
		{
			if (IsSubscribed)
				return;

			var proxy = await GetProxy().ConfigureAwait(false);          

			proxy.On<TextMoodModel>(SignalRConstants.SendNewTextMoodModelName, textMoodModel =>
			{
				var textResultsListViewModel = GetTextResultsListViewModel();
				textResultsListViewModel?.AddTextMoodModelCommand?.Execute(textMoodModel);
			});
		}

		static TextResultsListViewModel GetTextResultsListViewModel()
		{
			var navigationPage = Application.Current.MainPage as NavigationPage;
			return navigationPage.RootPage.BindingContext as TextResultsListViewModel;         
		}

		static bool GetIsSubscribed()
		{
			var hub = GetHubConnection();
			return hub.State.Equals(ConnectionState.Connected);
		}
	}
}
