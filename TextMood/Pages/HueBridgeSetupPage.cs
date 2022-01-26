using System;
using Xamarin.CommunityToolkit.Markup;
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

            Title = "Configure Bridge";

            Content = new StackLayout
            {
                Children =
                {
                    new Label { Text = "Philips Hue Bridge ID" },

                    new Entry { Placeholder = "Eg: 001788fffe75a1d2" }.Assign(out _bridgeIDEntry)
                        .Bind(Entry.TextProperty, nameof(BindingContext.BridgeIDEntryText))
                        .Bind(IsEnabledProperty, nameof(BindingContext.AreEntriesEnabled)),

                    new Label { Text = "Philips Hue Bridge IP Address" },

                    new Entry
                    {
                        Placeholder = "0.0.0.0",
                        Keyboard = Device.RuntimePlatform is Device.iOS ? Keyboard.Numeric : Keyboard.Default,
                    }.Assign(out _bridgeIPEntry)
                     .Bind(Entry.TextProperty, nameof(BindingContext.BridgeIPEntryText))
                     .Bind(IsEnabledProperty, nameof(BindingContext.AreEntriesEnabled)),

                    new StackLayout
                    {
                        Orientation = StackOrientation.Horizontal,
                        Children =
                        {
                            new Label { Text = "Enable Philips Hue Bridge" }.TextCenter(),
                            new Switch().Assign(out _isBridgeConnectedSwitch)
                                .Bind(Switch.IsToggledProperty, nameof(BindingContext.IsBridgeConnectedSwitchToggled))
                        }
                    }.Center().Margins(0, 10, 0, 0),

                    new Button { Text = "Auto Detect" }.Margin(0, 10)
                        .Bind(Button.CommandProperty, nameof(BindingContext.AutoDetectButtonCommand))
                        .Bind(IsEnabledProperty, nameof(BindingContext.AreEntriesEnabled)),

                    new Button { Text = "Save" }
                        .Bind(IsEnabledProperty, nameof(BindingContext.IsSaveButtonEnabled))
                        .Bind(Button.CommandProperty, nameof(BindingContext.SaveButtonCommand)),

                    new Button { Text = "Cancel" }
                        .Invoke(cancelButton => cancelButton.Clicked += HandleCancelButtonClicked),

                    new ActivityIndicator()
                        .Bind(IsVisibleProperty, nameof(BindingContext.IsActivityIndicatorVisible))
                        .Bind(ActivityIndicator.IsRunningProperty, nameof(BindingContext.IsActivityIndicatorVisible))
        }
            }.Center();
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
