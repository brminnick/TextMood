using System;
using System.Net;

namespace TextMood
{
    abstract class PhilipsHueBridgeSettings : BaseSettings
	{
		#region Fields
		static bool _isEnabled;
		static string _id, _username;
		static IPAddress _ipAddress;
        #endregion

        #region Events
        public static event EventHandler<IPAddress> IPAddressChanged;
        #endregion

        #region Properties
        public static IPAddress IPAddress
		{
            get => GetSetting(ref _ipAddress);
            set => SetSetting(ref _ipAddress, value, OnIPAddressChanged);
		}

        public static bool IsEnabled
		{
			get => GetSetting(ref _isEnabled, true);
			set => SetSetting(ref _isEnabled, value);
		}

		public static string Id
		{
			get => GetSetting(ref _id);
			set => SetSetting(ref _id, value);
		}

		public static string Username
		{
			get => GetSetting(ref _username);
			set => SetSetting(ref _username, value);         
		}

        static void OnIPAddressChanged() => IPAddressChanged?.Invoke(null, IPAddress);
        #endregion
    }
}
