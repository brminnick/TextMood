using System.Data;
using System.Threading.Tasks;

using Microsoft.AspNetCore.SignalR.Client;

using TextMood.Shared;

using Xamarin.Forms;

namespace TextMood
{
	abstract class SignalRService : BaseSignalRService
	{
		public static async Task Subscribe()
		{
			if (HubConnectionState.Equals(ConnectionState.Open))
				return;

            var connection = await GetConnection().ConfigureAwait(false);

			connection.On<TextMoodModel>(SignalRConstants.SendNewTextMoodModelName, textMoodModel =>
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
	}
}
