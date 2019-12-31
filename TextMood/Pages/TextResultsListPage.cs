using System;
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

            var textModelList = new ListView(ListViewCachingStrategy.RecycleElement)
            {
                ItemTemplate = new DataTemplate(typeof(TextMoodViewCell)),
                IsPullToRefreshEnabled = true,
                HasUnevenRows = true,
                BackgroundColor = Color.Transparent,
                RefreshControlColor = Device.RuntimePlatform is Device.iOS ? ColorConstants.BarTextColor : ColorConstants.BarBackgroundColor
            };
            textModelList.ItemTapped += HandleItemTapped;
            textModelList.SetBinding(ListView.IsRefreshingProperty, nameof(TextResultsListViewModel.IsRefreshing));
            textModelList.SetBinding(ListView.ItemsSourceProperty, nameof(TextResultsListViewModel.TextList));
            textModelList.SetBinding(ListView.RefreshCommandProperty, nameof(TextResultsListViewModel.PullToRefreshCommand));

            Title = PageTitles.TextResultsPage;

            this.SetBinding(BackgroundColorProperty, nameof(TextResultsListViewModel.BackgroundColor));

            Content = textModelList;
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();

            if(Content is ListView listView)
                Device.BeginInvokeOnMainThread(listView.BeginRefresh);

            await SignalRService.Subscribe().ConfigureAwait(false);
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

        void HandleItemTapped(object sender, ItemTappedEventArgs e)
        {
            if (sender is ListView listView)
                listView.SelectedItem = null;
        }
    }
}
