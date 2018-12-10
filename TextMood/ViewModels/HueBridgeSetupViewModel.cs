using System;
using System.Globalization;
using System.Net;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Input;
using AsyncAwaitBestPractices.MVVM;

namespace TextMood
{
    public class HueBridgeSetupViewModel : BaseViewModel
    {
        #region Constant Fields
        const string _bridgeNotFoundErrorMessage = "Bridge Not Found";
        #endregion

        #region Fields
        bool _isBridgeConnectedSwitchToggled, _isActivityIndicatorVisible;
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
            (_saveButtonCommand = new AsyncCommand(() => ExecuteSaveButtonCommand(BridgeIPEntryText, BridgeIDEntryText), continueOnCapturedContext: false));

        public ICommand AutoDetectButtonCommand => _autoDetectButtonCommand ??
            (_autoDetectButtonCommand = new AsyncCommand(ExecuteAutoDetectButtonCommand, continueOnCapturedContext: false));

        public bool IsSaveButtonEnabled => !IsBridgeConnectedSwitchToggled || (IsValidID(BridgeIDEntryText) && IsValidIPAddress(BridgeIPEntryText) && !IsActivityIndicatorVisible);

        public bool AreEntriesEnabled => !IsActivityIndicatorVisible && IsBridgeConnectedSwitchToggled;

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

        public bool IsActivityIndicatorVisible
        {
            get => _isActivityIndicatorVisible;
            set => SetProperty(ref _isActivityIndicatorVisible, value, NotifyIsActivityIndicatorVisibleProperties);
        }

        public bool IsBridgeConnectedSwitchToggled
        {
            get => _isBridgeConnectedSwitchToggled;
            set => SetProperty(ref _isBridgeConnectedSwitchToggled, value, NotifyIsBridgeConnectedSwitchToggledProperties);
        }
        #endregion

        #region Methods
        async Task ExecuteAutoDetectButtonCommand()
        {
            IsActivityIndicatorVisible = true;

            try
            {
                var autoDetectedBridgeList = await PhilipsHueServices.AutoDetectBridges().ConfigureAwait(false);

                foreach (var bridge in autoDetectedBridgeList)
                {
                    if (IsValidIPAddress(bridge.InternalIPAddress))
                    {
                        BridgeIDEntryText = bridge.Id;
                        BridgeIPEntryText = bridge.InternalIPAddress;
                        OnAutoDiscoveryCompleted("Bridge Detected");
                        return;
                    }
                }

                OnAutoDiscoveryCompleted(_bridgeNotFoundErrorMessage);
            }
            catch (Exception e)
            {
                DebugServices.Report(e);

                BridgeIPEntryText = string.Empty;
                OnAutoDiscoveryCompleted(_bridgeNotFoundErrorMessage);
            }
            finally
            {
                IsActivityIndicatorVisible = false;
            }
        }

        async Task ExecuteSaveButtonCommand(string philipsHueBridgeIPAddress, string philipsHueBridgeID)
        {
            IsActivityIndicatorVisible = true;

            try
            {
                PhilipsHueBridgeSettings.IsEnabled = IsBridgeConnectedSwitchToggled;

                if (IsBridgeConnectedSwitchToggled)
                {
                    PhilipsHueBridgeSettings.IPAddress = IPAddress.Parse(philipsHueBridgeIPAddress);

                    var usernameResponseList = await PhilipsHueServices.AutoDetectUsername().ConfigureAwait(false);
                    if (usernameResponseList is null)
                    {
                        OnSaveFailed(_bridgeNotFoundErrorMessage);
                        return;
                    }

                    foreach (var usernameResponse in usernameResponseList)
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
                            OnSaveCompleted();
                            return;
                        }
                    }
                }
                else
                {
                    OnSaveCompleted();
                }
            }
            catch (Exception e)
            {
                DebugServices.Report(e);

                OnSaveFailed(e.Message);
            }
            finally
            {
                IsActivityIndicatorVisible = false;
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

        void NotifyIsBridgeConnectedSwitchToggledProperties()
        {
            OnPropertyChanged(nameof(AreEntriesEnabled));
            OnPropertyChanged(nameof(IsSaveButtonEnabled));
        }

        void NotifyIsActivityIndicatorVisibleProperties()
        {
            OnPropertyChanged(nameof(AreEntriesEnabled));
            OnPropertyChanged(nameof(IsSaveButtonEnabled));
        }

        bool IsValidIPAddress(string text) => IPAddress.TryParse(text, out _);
        void OnSaveFailed(string message) => SaveFailed?.Invoke(this, message);
        void OnSaveCompleted() => SaveCompleted?.Invoke(this, EventArgs.Empty);
        void OnAutoDiscoveryCompleted(string message) => AutoDiscoveryCompleted?.Invoke(this, message);
        #endregion
    }
}
