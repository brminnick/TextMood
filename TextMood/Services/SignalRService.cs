using System.Threading.Tasks;
using Microsoft.AspNet.SignalR.Client;
using Xamarin.Forms;
using System.Collections.Generic;

namespace TextMood
{
	public abstract class SignalRService : BaseSignalRService
	{
        public static async Task Subscribe()
		{
			var proxy = await GetProxy().ConfigureAwait(false);

			proxy.On<TextMoodModel>("SendNewTextMoodModel", textMoodModel =>
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
