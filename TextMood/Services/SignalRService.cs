using System.Threading.Tasks;

using Microsoft.AspNetCore.SignalR.Client;

using TextMood.Shared;

using Xamarin.Forms;

namespace TextMood
{
    public abstract class SignalRService : BaseSignalRService
    {
        public static async Task Subscribe()
        {
            if (HubConnectionState is HubConnectionState.Connected)
                return;

            var connection = await GetConnection().ConfigureAwait(false);

            connection.On<TextMoodModel>(SignalRConstants.SendNewTextMoodModelName, textMoodModel =>
            {
                var textResultsListViewModel = GetTextResultsListViewModel();
                textResultsListViewModel.AddTextMoodModelCommand.Execute(textMoodModel);
            });
        }

        static TextResultsListViewModel GetTextResultsListViewModel()
        {
            var navigationPage = (NavigationPage)Application.Current.MainPage;
            return (TextResultsListViewModel)navigationPage.RootPage.BindingContext;
        }
    }
}
