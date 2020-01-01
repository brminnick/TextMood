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
            var connection = await GetConnection().ConfigureAwait(false);

            connection.On<TextMoodModel>(SignalRConstants.SendNewTextMoodModelName, async textMoodModel =>
            {
                await GetTextResultsListViewModel().AddTextMoodModel(textMoodModel).ConfigureAwait(false);

                if (GetTextResultsListPage().Content is RefreshView refreshView
                    && refreshView.Content is CollectionView collectionView)
                {
                    await Device.InvokeOnMainThreadAsync(() => collectionView.ScrollTo(0)).ConfigureAwait(false);
                }
            });
        }

        static TextResultsListPage GetTextResultsListPage()
        {
            var navigationPage = (NavigationPage)Application.Current.MainPage;
            return (TextResultsListPage)navigationPage.RootPage;
        }

        static TextResultsListViewModel GetTextResultsListViewModel() => (TextResultsListViewModel)GetTextResultsListPage().BindingContext;
    }
}
