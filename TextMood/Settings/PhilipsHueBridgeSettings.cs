using System;
using System.Net;
using AsyncAwaitBestPractices;
using Xamarin.Essentials;

namespace TextMood
{
    static class PhilipsHueBridgeSettings
    {
        readonly static WeakEventManager<IPAddress> _ipAddressChangedEventManager = new WeakEventManager<IPAddress>();

        public static event EventHandler<IPAddress> IPAddressChanged
        {
            add => _ipAddressChangedEventManager.AddEventHandler(value);
            remove => _ipAddressChangedEventManager.RemoveEventHandler(value);
        }

        public static IPAddress IPAddress
        {
            get => IPAddress.Parse(Preferences.Get(nameof(IPAddress), "0.0.0.0"));
            set
            {
                if (value != IPAddress)
                {
                    Preferences.Set(nameof(IPAddress), value.ToString());
                    OnIPAddressChanged();
                }
            }
        }

        public static bool IsEnabled
        {
            get => Preferences.Get(nameof(IsEnabled), true);
            set => Preferences.Set(nameof(IsEnabled), value);
        }

        public static string Id
        {
            get => Preferences.Get(nameof(Id), string.Empty);
            set => Preferences.Set(nameof(Id), value);
        }

        public static string Username
        {
            get => Preferences.Get(nameof(Username), string.Empty);
            set => Preferences.Set(nameof(Username), value);
        }

        static void OnIPAddressChanged() => _ipAddressChangedEventManager.HandleEvent(null, IPAddress, nameof(IPAddressChanged));
    }
}
