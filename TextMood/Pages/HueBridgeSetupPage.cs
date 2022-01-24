using System;
using Xamarin.Essentials.Interfaces;
using Xamarin.Forms;

namespace TextMood
{
    public class HueBridgeSetupPage : BaseContentPage<HueBridgeSetupViewModel>
    {
        readonly IMainThread _mainThread;
        readonly PhilipsHueBridgeSettingsService _philipsHueBridgeSettingsService;
        readonly Switch _isBridgeConnectedSwitch;
        readonly Entry _bridgeIPEntry, _bridgeIDEntry;

        public HueBridgeSetupPage(IMainThread mainThread,
                                    HueBridgeSetupViewModel hueBridgeSetupViewModel,
                                    PhilipsHueBridgeSettingsService philipsHueBridgeSettingsService) : base(hueBridgeSetupViewModel)
        {
            _mainThread = mainThread;
            _philipsHueBridgeSettingsService = philipsHueBridgeSettingsService;

            BindingContext.SaveFailed += HandleSaveFailed;
            BindingContext.SaveCompleted += HandleSaveCompleted;
            BindingContext.AutoDiscoveryCompleted += HandleAutoDiscoveryCompleted;

            var bridgeIDLabel = new Label { Text = "Philips Hue Bridge ID" };

            _bridgeIDEntry = new Entry { Placeholder = "Eg: 001788fffe75a1d2" };
            _bridgeIDEntry.SetBinding(Entry.TextProperty, nameof(BindingContext.BridgeIDEntryText));
            _bridgeIDEntry.SetBinding(IsEnabledProperty, nameof(BindingContext.AreEntriesEnabled));

            var bridgeIPLabel = new Label { Text = "Philips Hue Bridge IP Address" };

            _bridgeIPEntry = new Entry
            {
                Placeholder = "0.0.0.0",
                Keyboard = Device.RuntimePlatform is Device.iOS ? Keyboard.Numeric : Keyboard.Default,
            };
            _bridgeIPEntry.SetBinding(Entry.TextProperty, nameof(BindingContext.BridgeIPEntryText));
            _bridgeIPEntry.SetBinding(IsEnabledProperty, nameof(BindingContext.AreEntriesEnabled));

            var isBridgeConnectedLabel = new Label
            {
                Text = "Enable Philips Hue Bridge",
                HorizontalTextAlignment = TextAlignment.Center,
                VerticalTextAlignment = TextAlignment.Center
            };

            _isBridgeConnectedSwitch = new Switch();
            _isBridgeConnectedSwitch.SetBinding(Switch.IsToggledProperty, nameof(BindingContext.IsBridgeConnectedSwitchToggled));

            var switchStackLayout = new StackLayout
            {
                Orientation = StackOrientation.Horizontal,
                HorizontalOptions = LayoutOptions.Center,
                VerticalOptions = LayoutOptions.Center,
                Margin = new Thickness(0, 10, 0, 0),
                Children = { isBridgeConnectedLabel, _isBridgeConnectedSwitch }
            };

            var autoDetectButton = new Button
            {
                Text = "Auto Detect",
                Margin = new Thickness(0, 10)
            };
            autoDetectButton.SetBinding(Button.CommandProperty, nameof(BindingContext.AutoDetectButtonCommand));
            autoDetectButton.SetBinding(IsEnabledProperty, nameof(BindingContext.AreEntriesEnabled));

            var saveButton = new Button { Text = "Save" };
            saveButton.SetBinding(IsEnabledProperty, nameof(BindingContext.IsSaveButtonEnabled));
            saveButton.SetBinding(Button.CommandProperty, nameof(BindingContext.SaveButtonCommand));

            var cancelButton = new Button { Text = "Cancel" };
            cancelButton.Clicked += HandleCancelButtonClicked;

            var activityIndicator = new ActivityIndicator { InputTransparent = true };
            activityIndicator.SetBinding(IsVisibleProperty, nameof(BindingContext.IsActivityIndicatorVisible));
            activityIndicator.SetBinding(ActivityIndicator.IsRunningProperty, nameof(BindingContext.IsActivityIndicatorVisible));

            Title = "Configure Bridge";

            var stackLayout = new StackLayout
            {
                HorizontalOptions = LayoutOptions.Center,
                VerticalOptions = LayoutOptions.Center,
                Children = {
                    bridgeIDLabel,
                    _bridgeIDEntry,
                    bridgeIPLabel,
                    _bridgeIPEntry,
                    switchStackLayout,
                    autoDetectButton,
                    saveButton,
                    cancelButton
                }
            };

            var absoluteLayout = new AbsoluteLayout();
            absoluteLayout.Children.Add(stackLayout, new Rectangle(.5, .5, -1, -1), AbsoluteLayoutFlags.PositionProportional);
            absoluteLayout.Children.Add(activityIndicator, new Rectangle(.5, .5, -1, -1), AbsoluteLayoutFlags.PositionProportional);

            Content = absoluteLayout;
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();

            _bridgeIDEntry.Text = _philipsHueBridgeSettingsService.Id;
            _bridgeIPEntry.Text = _philipsHueBridgeSettingsService.IPAddress.ToString();
            _isBridgeConnectedSwitch.IsToggled = _philipsHueBridgeSettingsService.IsEnabled;
        }

        void HandleSaveFailed(object sender, string message) =>
            _mainThread.BeginInvokeOnMainThread(async () => await DisplayAlert("Save Failed", message, "OK"));

        void HandleSaveCompleted(object sender, EventArgs e)
        {
            _mainThread.BeginInvokeOnMainThread(async () =>
            {
                await DisplayAlert("Bridge Settings Updated", "", "OK");
                ClosePage();
            });
        }

        void HandleAutoDiscoveryCompleted(object sender, string message) =>
            _mainThread.BeginInvokeOnMainThread(async () => await DisplayAlert("Auto Discovery Completed", message, "OK"));

        void HandleCancelButtonClicked(object sender, EventArgs e) => ClosePage();

        void ClosePage() => _mainThread.BeginInvokeOnMainThread(async () => await Navigation.PopModalAsync());
    }
}
