using System;
using System.Net;
using AsyncAwaitBestPractices;
using Xamarin.Essentials.Interfaces;

namespace TextMood
{
    public class PhilipsHueBridgeSettingsService
    {
        readonly WeakEventManager<IPAddress> _ipAddressChangedEventManager = new WeakEventManager<IPAddress>();
        readonly IPreferences _preferences;

        public PhilipsHueBridgeSettingsService(IPreferences preferences) => _preferences = preferences;

        public event EventHandler<IPAddress> IPAddressChanged
        {
            add => _ipAddressChangedEventManager.AddEventHandler(value);
            remove => _ipAddressChangedEventManager.RemoveEventHandler(value);
        }

        public IPAddress IPAddress
        {
            get => IPAddress.Parse(_preferences.Get(nameof(IPAddress), "0.0.0.0"));
            set
            {
                if (value != IPAddress)
                {
                    _preferences.Set(nameof(IPAddress), value.ToString());
                    OnIPAddressChanged();
                }
            }
        }

        public bool IsEnabled
        {
            get => _preferences.Get(nameof(IsEnabled), true);
            set => _preferences.Set(nameof(IsEnabled), value);
        }

        public string Id
        {
            get => _preferences.Get(nameof(Id), string.Empty);
            set => _preferences.Set(nameof(Id), value);
        }

        public string Username
        {
            get => _preferences.Get(nameof(Username), string.Empty);
            set => _preferences.Set(nameof(Username), value);
        }

        void OnIPAddressChanged() => _ipAddressChangedEventManager.RaiseEvent(null, IPAddress, nameof(IPAddressChanged));
    }
}
