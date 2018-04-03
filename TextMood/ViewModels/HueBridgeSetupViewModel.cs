using System;
using System.Globalization;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Input;

using Xamarin.Forms;

namespace TextMood
{
	public class HueBridgeSetupViewModel : BaseViewModel
	{
		#region Fields
		bool _areEntriesEnabled = true;
		string _bridgeIDEntryText, _bridgeIPEntryText;
		ICommand _autoDetectButtonCommand, _saveButtonCommand;
		#endregion

		#region Events
		public event EventHandler SaveCompleted;
		public event EventHandler<string> SaveFailed;
		public event EventHandler<string> AutoDiscoveryCompleted;
		#endregion

		#region Properties
		public ICommand SaveButtonCommand => _saveButtonCommand ??
			(_saveButtonCommand = new Command(async () => await ExecuteSaveButtonCommand(BridgeIPEntryText, BridgeIDEntryText)));

		public ICommand AutoDetectButtonCommand => _autoDetectButtonCommand ??
			(_autoDetectButtonCommand = new Command(async () => await ExecuteAutoDetectButtonCommand()));

		public bool IsSaveButtonEnabled => IsValidID(BridgeIDEntryText) && IsValidIPAddress(BridgeIPEntryText);

		public string BridgeIDEntryText
		{
			get => _bridgeIDEntryText;
			set => SetProperty(ref _bridgeIDEntryText, value, () => OnPropertyChanged(nameof(IsSaveButtonEnabled)));
		}

		public string BridgeIPEntryText
		{
			get => _bridgeIPEntryText;
			set => SetProperty(ref _bridgeIPEntryText, value, () => OnPropertyChanged(nameof(IsSaveButtonEnabled)));
		}

		public bool AreEntriesEnabled
		{
			get => _areEntriesEnabled;
			set => SetProperty(ref _areEntriesEnabled, value);
		}
		#endregion

		#region Methods
		async Task ExecuteAutoDetectButtonCommand()
		{
			AreEntriesEnabled = false;

			try
			{
				var autoDetectedBridgeList = await PhilipsHueBridgeAPIServices.AutoDetectBridges().ConfigureAwait(false);

				foreach (var bridge in autoDetectedBridgeList)
				{
					if (IsValidIPAddress(bridge.InternalIPAddress))
					{
						BridgeIDEntryText = bridge.Id;
						BridgeIPEntryText = bridge.InternalIPAddress;
						OnAutoDiscoveryCompleted($"Bridge Detected");
						return;
					}
				}

				OnAutoDiscoveryCompleted($"Bridge Not Found");
			}
			catch
			{
				BridgeIPEntryText = string.Empty;
				OnAutoDiscoveryCompleted($"Bridge Not Found");
			}
			finally
			{
				AreEntriesEnabled = true;
			}
		}

		async Task ExecuteSaveButtonCommand(string philipsHueBridgeIPAddress, string philipsHueBridgeID)
		{
			AreEntriesEnabled = false;

			try
			{
				var usernameResponseList = await PhilipsHueBridgeAPIServices.AutoDetectUsername(philipsHueBridgeIPAddress).ConfigureAwait(false);

				foreach (var usernameResponse in usernameResponseList ?? new System.Collections.Generic.List<PhilipsHueUsernameDiscoveryModel>())
				{
					if (usernameResponse.Error != null)
					{
						var textInfo = new CultureInfo("en-US", false).TextInfo;
						OnSaveFailed($"{textInfo.ToTitleCase(usernameResponse.Error.Description)} for Bridge IP: {philipsHueBridgeIPAddress}");
						return;
					}

					if (usernameResponse.Success != null)
					{
						PhilipsHueBridgeSettings.Username = usernameResponse.Success.Username;
						PhilipsHueBridgeSettings.Id = philipsHueBridgeID;
						PhilipsHueBridgeSettings.IPAddress = philipsHueBridgeIPAddress;
						OnSaveCompleted();
						return;
					}
				}
			}
			catch (Exception e)
			{
				OnSaveFailed(e.Message);
			}
			finally
			{
				AreEntriesEnabled = true;
			}
		}

		bool IsValidID(string text)
		{
			try
			{
				return text.Length.Equals(16) && Regex.IsMatch(text, @"^[a-zA-Z0-9]+$");
			}
			catch
			{
				return false;
			}
		}

		bool IsValidIPAddress(string text)
		{
			try
			{
				var indexOfFirstDecimal = text.IndexOf('.');
				var indexOfSecondDecimal = text.IndexOf('.', indexOfFirstDecimal + 1);
				var indexOfThirdDecimal = text.IndexOf('.', indexOfSecondDecimal + 1);

				var firstOctetText = text.Substring(0, indexOfFirstDecimal);
				var isFirstOctectTextAnInt = int.TryParse(firstOctetText, out int firstOctetInt);

				var secondOctetText = text.Substring(indexOfFirstDecimal + 1, indexOfSecondDecimal - indexOfFirstDecimal - 1);
				var isSecondOctectTextAnInt = int.TryParse(secondOctetText, out int secondOctetInt);

				var thirdOctetText = text.Substring(indexOfSecondDecimal + 1, indexOfThirdDecimal - indexOfSecondDecimal - 1);
				var isThirdOctectTextAnInt = int.TryParse(thirdOctetText, out int thirdOctetInt);

				var fourthOctetText = text.Substring(indexOfThirdDecimal + 1);
				var isFourthOctectTextAnInt = int.TryParse(fourthOctetText, out int fourthOctetInt);

				if (!isFirstOctectTextAnInt || !isSecondOctectTextAnInt || !isThirdOctectTextAnInt || !isFourthOctectTextAnInt)
					return false;

				if (IsNumberBetween0And255(firstOctetInt) && IsNumberBetween0And255(secondOctetInt) && IsNumberBetween0And255(thirdOctetInt) && IsNumberBetween0And255(fourthOctetInt))
					return true;

				return false;
			}
			catch
			{
				return false;
			}

			bool IsNumberBetween0And255(int number) => number >= 0 && number <= 255;
		}

		void OnSaveFailed(string message) => SaveFailed?.Invoke(this, message);
		void OnSaveCompleted() => SaveCompleted?.Invoke(this, EventArgs.Empty);
		void OnAutoDiscoveryCompleted(string message) => AutoDiscoveryCompleted?.Invoke(this, message);
		#endregion
	}
}
