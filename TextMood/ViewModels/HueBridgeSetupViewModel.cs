using System;
using System.Globalization;
using System.Net;
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
						PhilipsHueBridgeSettings.IPAddress = IPAddress.Parse(philipsHueBridgeIPAddress);
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

		bool IsValidIPAddress(string text) => IPAddress.TryParse(text, out _);      
		void OnSaveFailed(string message) => SaveFailed?.Invoke(this, message);
		void OnSaveCompleted() => SaveCompleted?.Invoke(this, EventArgs.Empty);
		void OnAutoDiscoveryCompleted(string message) => AutoDiscoveryCompleted?.Invoke(this, message);
		#endregion
	}
}
