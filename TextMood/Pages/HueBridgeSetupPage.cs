using System;
using Xamarin.Forms;

namespace TextMood
{
	public class HueBridgeSetupPage : BaseContentPage<HueBridgeSetupViewModel>
	{
		readonly Entry _bridgeIPEntry, _bridgeIDEntry;
		readonly Button _saveButton, _cancelButton, _autoDetectButton;

		public HueBridgeSetupPage()
		{
			var bridgeIDLabel = new Label { Text = "Philips Hue Bridge ID" };

			_bridgeIDEntry = new Entry { Placeholder = "Eg: 001788fffe75a1d2" };
			_bridgeIDEntry.SetBinding(Entry.TextProperty, nameof(ViewModel.BridgeIDEntryText));
			_bridgeIDEntry.SetBinding(IsEnabledProperty, nameof(ViewModel.AreEntriesEnabled));

			var bridgeIPLabel = new Label { Text = "Philips Hue Bridge IP Address" };

			_bridgeIPEntry = new Entry
			{
				Placeholder = "0.0.0.0",
				Keyboard = Device.RuntimePlatform.Equals(Device.iOS) ? Keyboard.Numeric : Keyboard.Default,
			};
			_bridgeIPEntry.SetBinding(Entry.TextProperty, nameof(ViewModel.BridgeIPEntryText));
			_bridgeIPEntry.SetBinding(IsEnabledProperty, nameof(ViewModel.AreEntriesEnabled));

			_autoDetectButton = new Button
			{
				Text = "Auto Detect",
				Margin = new Thickness(0, 10)
			};
			_autoDetectButton.SetBinding(Button.CommandProperty, nameof(ViewModel.AutoDetectButtonCommand));

			_saveButton = new Button { Text = "Save" };
			_saveButton.SetBinding(IsEnabledProperty, nameof(ViewModel.IsSaveButtonEnabled));
			_saveButton.SetBinding(Button.CommandProperty, nameof(ViewModel.SaveButtonCommand));

			_cancelButton = new Button { Text = "Cancel" };

			var activityIndicator = new ActivityIndicator { InputTransparent = true };
			activityIndicator.SetBinding(IsVisibleProperty, new Binding(nameof(ViewModel.AreEntriesEnabled), BindingMode.Default, new InverseBooleanConverter(), ViewModel.AreEntriesEnabled));
			activityIndicator.SetBinding(ActivityIndicator.IsRunningProperty, new Binding(nameof(ViewModel.AreEntriesEnabled), BindingMode.Default, new InverseBooleanConverter(), ViewModel.AreEntriesEnabled));



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
					_autoDetectButton,
					_saveButton,
					_cancelButton
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

			_bridgeIDEntry.Text = PhilipsHueBridgeSettings.Id;
			_bridgeIPEntry.Text = PhilipsHueBridgeSettings.IPAddress.ToString();
		}

		protected override void SubscribeEventHandlers()
		{
			ViewModel.SaveFailed += HandleSaveFailed;
			ViewModel.SaveCompleted += HandleSaveCompleted;
			_cancelButton.Clicked += HandleCancelButtonClicked;
			ViewModel.AutoDiscoveryCompleted += HandleAutoDiscoveryCompleted;
		}

		protected override void UnsubscribeEventHandlers()
		{
			ViewModel.SaveFailed -= HandleSaveFailed;
			ViewModel.SaveCompleted -= HandleSaveCompleted;
			_cancelButton.Clicked -= HandleCancelButtonClicked;
			ViewModel.AutoDiscoveryCompleted -= HandleAutoDiscoveryCompleted;
		}

		void HandleSaveFailed(object sender, string message) =>
			Device.BeginInvokeOnMainThread(async () => await DisplayAlert("Save Failed", message, "OK"));

		void HandleSaveCompleted(object sender, EventArgs e)
		{
			Device.BeginInvokeOnMainThread(async () =>
			{
				await DisplayAlert("Bridge Saved", "", "OK");
				ClosePage();
			});
		}

		void HandleAutoDiscoveryCompleted(object sender, string message) =>
			Device.BeginInvokeOnMainThread(async () => await DisplayAlert("Auto Discovery Completed", message, "OK"));

		void HandleCancelButtonClicked(object sender, EventArgs e) => ClosePage();

		void ClosePage() => Device.BeginInvokeOnMainThread(async () => await Navigation.PopModalAsync());
	}
}
