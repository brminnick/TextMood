using System;
using System.Collections;
using System.Threading.Tasks;
using Xamarin.CommunityToolkit.Markup;
using Xamarin.Essentials.Interfaces;
using Xamarin.Forms;

namespace TextMood
{
    public class TextResultsListPage : BaseContentPage<TextResultsListViewModel>
    {
        readonly IMainThread _mainThread;
        readonly HueBridgeSetupPage _hueBridgeSetupPage;
        readonly PhilipsHueBridgeSettingsService _philipsHueBridgeSettingsService;

        public TextResultsListPage(IMainThread mainThread,
                                    SignalRService signalRService,
                                    HueBridgeSetupPage hueBridgeSetupPage,
                                    TextResultsListViewModel textResultsListViewModel,
                                    PhilipsHueBridgeSettingsService philipsHueBridgeSettingsService) : base(textResultsListViewModel)
        {
            _mainThread = mainThread;
            _hueBridgeSetupPage = hueBridgeSetupPage;
            _philipsHueBridgeSettingsService = philipsHueBridgeSettingsService;

            BindingContext.ErrorTriggered += HandleErrorTriggered;
            BindingContext.PhilipsHueBridgeConnectionFailed += HandlePhilipsHueBridgeConnectionFailed;
            signalRService.InitializationFailed += HandleInitializationFailed;

            ToolbarItems.Add(new ToolbarItem { Text = "Setup" }
                                .Invoke(setupToolbarItem => setupToolbarItem.Clicked += HandleSetupPageToolbarItemClicked));

            Title = PageTitles.TextResultsPage;

            this.SetBinding(BackgroundColorProperty, nameof(BindingContext.BackgroundColor));

            Content = new RefreshView
            {
                RefreshColor = Device.RuntimePlatform is Device.iOS ? ColorConstants.BarTextColor : ColorConstants.BarBackgroundColor,
                Content = new CollectionView
                {
                    ItemTemplate = new TextMoodDataTemplateSelector(),
                    BackgroundColor = Color.Transparent
                }.Bind(CollectionView.ItemsSourceProperty, nameof(BindingContext.TextList))

            }.Bind(RefreshView.IsRefreshingProperty, nameof(BindingContext.IsRefreshing))
             .Bind(RefreshView.CommandProperty, nameof(BindingContext.PullToRefreshCommand));
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

        void HandlePhilipsHueBridgeConnectionFailed(object sender, EventArgs e) => _mainThread.BeginInvokeOnMainThread(async () =>
        {
            var selectionResult = await DisplayAlert("Could Not Connect to Philips Hue Bridge", "Would you like to setup a bridge, or disable the connection?", "Setup Bridge", "Disable Bridge");

            if (selectionResult)
            {
                await NavigateToSetupPage();
            }
            else
            {
                _philipsHueBridgeSettingsService.IsEnabled = false;
                await DisplayAlert("Philips Hue Bridge Connection Disabled", "You can configure the bridge connection anytime by tapping \"Setup\"", "OK");
            }
        });

        async void HandleSetupPageToolbarItemClicked(object sender, EventArgs e) => await NavigateToSetupPage();

        async void HandleErrorTriggered(object sender, string message) => await DisplayErrorMessage(message);

        async void HandleInitializationFailed(object sender, string message) => await DisplayErrorMessage(message);

        Task NavigateToSetupPage() => Device.InvokeOnMainThreadAsync(() => Navigation.PushModalAsync(new BaseNavigationPage(_hueBridgeSetupPage)));

        Task DisplayErrorMessage(string message) => _mainThread.InvokeOnMainThreadAsync(() => DisplayAlert("Error", message, "OK "));
    }
}
