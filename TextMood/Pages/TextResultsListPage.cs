using System;
using System.Collections;
using System.Threading.Tasks;
using TextMood.Shared;
using Xamarin.Forms;

namespace TextMood
{
    public class TextResultsListPage : BaseContentPage<TextResultsListViewModel>
    {
        public TextResultsListPage()
        {
            ViewModel.ErrorTriggered += HandleErrorTriggered;
            ViewModel.PhilipsHueBridgeConnectionFailed += HandlePhilipsHueBridgeConnectionFailed;
            BaseSignalRService.InitializationFailed += HandleInitializationFailed;

            var setupPageToolbarItem = new ToolbarItem { Text = "Setup" };
            setupPageToolbarItem.Clicked += HandleSetupPageToolbarItemClicked;
            ToolbarItems.Add(setupPageToolbarItem);

            var textModelList = new CollectionView
            {
                ItemTemplate = new TextMoodDataTemplateSelector(),
                BackgroundColor = Color.Transparent
            };
            textModelList.SetBinding(CollectionView.ItemsSourceProperty, nameof(TextResultsListViewModel.TextList));

            var refreshView = new RefreshView
            {
                RefreshColor = Device.RuntimePlatform is Device.iOS ? ColorConstants.BarTextColor : ColorConstants.BarBackgroundColor,
                Content = textModelList
            };
            refreshView.SetBinding(RefreshView.IsRefreshingProperty, nameof(TextResultsListViewModel.IsRefreshing));
            refreshView.SetBinding(RefreshView.CommandProperty, nameof(TextResultsListViewModel.PullToRefreshCommand));

            Title = PageTitles.TextResultsPage;

            this.SetBinding(BackgroundColorProperty, nameof(TextResultsListViewModel.BackgroundColor));

            Content = refreshView;
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();

            if (Content is RefreshView refreshView
                && refreshView.Content is CollectionView collectionView
                && IsNullOrEmpty(collectionView.ItemsSource))
            {
                refreshView.IsRefreshing = true;
            }

            static bool IsNullOrEmpty(in IEnumerable? enumerable) => !enumerable?.GetEnumerator().MoveNext() ?? true;
        }

        void HandlePhilipsHueBridgeConnectionFailed(object sender, EventArgs e)
        {
            Device.BeginInvokeOnMainThread(async () =>
            {
                var selectionResult = await DisplayAlert("Could Not Connect to Philips Hue Bridge", "Would you like to setup a bridge, or disable the connection?", "Setup Bridge", "Disable Bridge");

                if (selectionResult)
                {
                    await NavigateToSetupPage();
                }
                else
                {
                    PhilipsHueBridgeSettings.IsEnabled = false;
                    await DisplayAlert("Philips Hue Bridge Connection Disabled", "You can configure the bridge connection anytime by tapping \"Setup\"", "OK");
                }
            });
        }

        async void HandleSetupPageToolbarItemClicked(object sender, EventArgs e) => await NavigateToSetupPage();

        async void HandleErrorTriggered(object sender, string message) => await DisplayErrorMessage(message);

        async void HandleInitializationFailed(object sender, string message) => await DisplayErrorMessage(message);

        Task NavigateToSetupPage() => Device.InvokeOnMainThreadAsync(() => Navigation.PushModalAsync(new BaseNavigationPage(new HueBridgeSetupPage())));

        Task DisplayErrorMessage(string message) => Device.InvokeOnMainThreadAsync(() => DisplayAlert("Error", message, "OK "));
    }
}
