using System;

using TextMood.Shared;

using Xamarin.Forms;

namespace TextMood
{
    public class TextResultsListPage : BaseContentPage<TextResultsListViewModel>
    {
        #region Constant Fields
        readonly ListView _textModelList;
        #endregion

        #region Constructors
        public TextResultsListPage()
        {
            ViewModel.ErrorTriggered += HandleErrorTriggered;
            ViewModel.PhilipsHueBridgeConnectionFailed += HandlePhilipsHueBridgeConnectionFailed;
            BaseSignalRService.InitializationFailed += HandleInitializationFailed;

            var setupPageToolbarItem = new ToolbarItem { Text = "Setup" };
            setupPageToolbarItem.Clicked += HandleSetupPageToolbarItemClicked;
            ToolbarItems.Add(setupPageToolbarItem);

            _textModelList = new ListView(ListViewCachingStrategy.RecycleElement)
            {
                ItemTemplate = new DataTemplate(typeof(TextMoodViewCell)),
                IsPullToRefreshEnabled = true,
                HasUnevenRows = true,
            };
            _textModelList.ItemTapped += HandleItemTapped;
            _textModelList.SetBinding(ListView.IsRefreshingProperty, nameof(ViewModel.IsRefreshing));
            _textModelList.SetBinding(ListView.ItemsSourceProperty, nameof(ViewModel.TextList));
            _textModelList.SetBinding(ListView.RefreshCommandProperty, nameof(ViewModel.PullToRefreshCommand));
            _textModelList.SetBinding(ListView.BackgroundColorProperty, nameof(ViewModel.BackgroundColor));

            Title = PageTitles.TextResultsPage;

            Content = _textModelList;
        }
        #endregion

        #region Methods
        protected override async void OnAppearing()
        {
            base.OnAppearing();

            Device.BeginInvokeOnMainThread(_textModelList.BeginRefresh);

            await SignalRService.Subscribe().ConfigureAwait(false);
        }

        void HandlePhilipsHueBridgeConnectionFailed(object sender, EventArgs e)
        {
            Device.BeginInvokeOnMainThread(async () =>
            {
                var selectionResult = await DisplayAlert("Could Not Connect to Philips Hue Bridge", "Would you like to setup a bridge, or disable the connection?", "Setup Bridge", "Disable Bridge");

                if (selectionResult)
                {
                    NavigateToSetupPage();
                }
                else
                {
                    PhilipsHueBridgeSettings.IsEnabled = false;
                    await DisplayAlert("Philips Hue Bridge Connection Disabled", "You can configure the bridge connection anytime by tapping \"Setup\"", "OK");
                }
            });
        }

        void HandleSetupPageToolbarItemClicked(object sender, EventArgs e) => NavigateToSetupPage();

        void NavigateToSetupPage() => Device.BeginInvokeOnMainThread(async () => await Navigation.PushModalAsync(new BaseNavigationPage(new HueBridgeSetupPage())));

        void HandleErrorTriggered(object sender, string message) => DisplayErrorMessage(message);

        void HandleInitializationFailed(object sender, string message) => DisplayErrorMessage(message);

        void DisplayErrorMessage(string message) => Device.BeginInvokeOnMainThread(async () => await DisplayAlert("Error", message, "OK "));

        void HandleItemTapped(object sender, ItemTappedEventArgs e)
        {
            var listView = sender as ListView;
            listView.SelectedItem = null;
        }
        #endregion
    }
}
