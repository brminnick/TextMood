using System;
using Xamarin.Forms;

namespace TextMood
{
	public class HueBridgeSetupPage : BaseContentPage<HueBridgeSetupViewModel>
	{
		readonly Entry _bridgeIPEntry;
		readonly Button _saveButton, _cancelButton;

		public HueBridgeSetupPage()
		{
			var bridgeIPLabel = new Label { Text = "Phillips Hue Bridge IP Address" };
			_bridgeIPEntry = new Entry
			{
				Placeholder = "0.0.0.0",
				Keyboard = Device.RuntimePlatform.Equals(Device.iOS) ? Keyboard.Numeric : Keyboard.Default,
			};
			_bridgeIPEntry.SetBinding(Entry.TextProperty, nameof(ViewModel.BridgeIPEntryText));

			_saveButton = new Button
			{
				Text = "Save",
				Margin = new Thickness(0, 10, 0, 0)
			};
			_saveButton.SetBinding(IsEnabledProperty, nameof(ViewModel.IsSaveButtonEnabled));

			_cancelButton = new Button { Text = "Cancel" };

			Title = "Configure Bridge";

			Content = new StackLayout
			{
				HorizontalOptions = LayoutOptions.Center,
				VerticalOptions = LayoutOptions.Center,
				Children = {
					bridgeIPLabel,
					_bridgeIPEntry,
					_saveButton,
					_cancelButton
				}
			};
		}

		protected override void SubscribeEventHandlers()
		{
			_saveButton.Clicked += HandleSaveToolBarItemClicked;
			_cancelButton.Clicked += HandleCancelToolBarItemClicked;
		}

		protected override void UnsubscribeEventHandlers()
		{
			_saveButton.Clicked -= HandleSaveToolBarItemClicked;
			_cancelButton.Clicked -= HandleCancelToolBarItemClicked;
		}

		void HandleSaveToolBarItemClicked(object sender, EventArgs e)
		{
			PhillipsHueBridgeServices.PhillipsHueBridgeIPAddress = _bridgeIPEntry.Text;
			ClosePage();
		}

		void HandleCancelToolBarItemClicked(object sender, EventArgs e) => ClosePage();

		void ClosePage() =>
			Device.BeginInvokeOnMainThread(async () => await Navigation.PopModalAsync());


	}
}
