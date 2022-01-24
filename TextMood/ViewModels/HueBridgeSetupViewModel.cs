using System;
using System.Globalization;
using System.Net;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Input;
using AsyncAwaitBestPractices;
using AsyncAwaitBestPractices.MVVM;

namespace TextMood
{
    public class HueBridgeSetupViewModel : BaseViewModel
    {
        const string _bridgeNotFoundErrorMessage = "Bridge Not Found";

        readonly WeakEventManager _saveCompletedEventManager = new WeakEventManager();
        readonly WeakEventManager<string> _saveFailedEventManager = new WeakEventManager<string>();
        readonly WeakEventManager<string> _autoDiscoveryCompletedEventManager = new WeakEventManager<string>();

        readonly PhilipsHueServices _philipsHueServices;
        readonly PhilipsHueBridgeSettingsService _philipsHueBridgeSettingsService;

        bool _isBridgeConnectedSwitchToggled, _isActivityIndicatorVisible;
        string _bridgeIDEntryText = string.Empty, _bridgeIPEntryText = string.Empty;

        public HueBridgeSetupViewModel(PhilipsHueServices philipsHueServices,
                                        PhilipsHueBridgeSettingsService philipsHueBridgeSettingsService)
        {
            _philipsHueServices = philipsHueServices;
            _philipsHueBridgeSettingsService = philipsHueBridgeSettingsService;

            SaveButtonCommand = new AsyncCommand(() => ExecuteSaveButtonCommand(BridgeIPEntryText, BridgeIDEntryText));
            AutoDetectButtonCommand = new AsyncCommand(ExecuteAutoDetectButtonCommand);
        }

        public event EventHandler SaveCompleted
        {
            add => _saveCompletedEventManager.AddEventHandler(value);
            remove => _saveCompletedEventManager.RemoveEventHandler(value);
        }

        public event EventHandler<string> SaveFailed
        {
            add => _saveFailedEventManager.AddEventHandler(value);
            remove => _saveFailedEventManager.RemoveEventHandler(value);
        }

        public event EventHandler<string> AutoDiscoveryCompleted
        {
            add => _autoDiscoveryCompletedEventManager.AddEventHandler(value);
            remove => _autoDiscoveryCompletedEventManager.RemoveEventHandler(value);
        }

        public IAsyncCommand SaveButtonCommand { get; }
        public IAsyncCommand AutoDetectButtonCommand { get; }

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

        async Task ExecuteAutoDetectButtonCommand()
        {
            IsActivityIndicatorVisible = true;

            try
            {
                var autoDetectedBridgeList = await _philipsHueServices.AutoDetectBridges().ConfigureAwait(false);

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
                _philipsHueBridgeSettingsService.IsEnabled = IsBridgeConnectedSwitchToggled;

                if (IsBridgeConnectedSwitchToggled)
                {
                    _philipsHueBridgeSettingsService.IPAddress = IPAddress.Parse(philipsHueBridgeIPAddress);

                    var usernameResponseList = await _philipsHueServices.AutoDetectUsername().ConfigureAwait(false);
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
                            _philipsHueBridgeSettingsService.Username = usernameResponse.Success.Username;
                            _philipsHueBridgeSettingsService.Id = philipsHueBridgeID;

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
        void OnSaveFailed(string message) => _saveFailedEventManager.RaiseEvent(this, message, nameof(SaveFailed));
        void OnSaveCompleted() => _saveCompletedEventManager.RaiseEvent(this, EventArgs.Empty, nameof(SaveCompleted));
        void OnAutoDiscoveryCompleted(string message) => _autoDiscoveryCompletedEventManager.RaiseEvent(this, message, nameof(AutoDiscoveryCompleted));
    }
}
