using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR.Client;
using TextMood.Shared;
using Xamarin.Essentials.Interfaces;
using Xamarin.Forms;

namespace TextMood
{
    public class SignalRService : BaseSignalRService
    {
        readonly IMainThread _mainThread;

        public SignalRService(IMainThread mainThread)
        {
            _mainThread = mainThread;
        }

        public async Task Subscribe()
        {
            var connection = await GetConnection().ConfigureAwait(false);

            connection.On<TextMoodModel>(SignalRConstants.SendNewTextMoodModelCommand, async textMoodModel =>
            {
                await GetTextResultsListViewModel().AddTextMoodModel(textMoodModel).ConfigureAwait(false);

                var refreshView = (RefreshView)GetTextResultsListPage().Content;
                var collectionView = (CollectionView)refreshView.Content;

                await _mainThread.InvokeOnMainThreadAsync(() => collectionView.ScrollTo(0)).ConfigureAwait(false);
            });
        }

        TextResultsListPage GetTextResultsListPage()
        {
            var navigationPage = (NavigationPage)Application.Current.MainPage;
            return (TextResultsListPage)navigationPage.RootPage;
        }

        TextResultsListViewModel GetTextResultsListViewModel() => GetTextResultsListPage().BindingContext;
    }
}
