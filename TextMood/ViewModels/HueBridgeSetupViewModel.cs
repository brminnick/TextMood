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
			(_saveButtonCommand = new Command(async () => await ExecuteSaveButtonCommand()));

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
				var autoDetectedBridgeList = await PhillipsHueBridgeServices.AutoDetectBridges().ConfigureAwait(false);

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
			catch (Exception)
			{
				BridgeIPEntryText = string.Empty;
				OnAutoDiscoveryCompleted($"Bridge Not Found");
			}
			finally
			{
				AreEntriesEnabled = true;
			}
		}

		async Task ExecuteSaveButtonCommand()
		{
			AreEntriesEnabled = false;

			try
			{
				var userNameResponseList = await PhillipsHueBridgeServices.AutoDetectUserName().ConfigureAwait(false);

				foreach (var userNameResponse in userNameResponseList)
				{
					if (userNameResponse.Error != null)
					{
						var textInfo = new CultureInfo("en-US", false).TextInfo;
						OnSaveFailed($"{textInfo.ToTitleCase(userNameResponse.Error.Description)} for Bridge IP: {PhillipsHueBridgeServices.PhillipsHueBridgeIPAddress}");
						return;
					}

					if (userNameResponse.Success != null)
					{
						PhillipsHueBridgeServices.PhillipsBridgeUserName = userNameResponse.Success.Username;
						PhillipsHueBridgeServices.PhillipsHueBridgeID = BridgeIDEntryText;
						PhillipsHueBridgeServices.PhillipsHueBridgeIPAddress = BridgeIPEntryText;
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
			catch (Exception)
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
			catch (Exception)
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
