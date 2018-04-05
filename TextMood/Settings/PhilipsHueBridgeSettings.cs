using System.Collections.Generic;
using System.Net;
using System.Runtime.CompilerServices;

using Plugin.Settings;

namespace TextMood
{
	public static class PhilipsHueBridgeSettings
	{
		#region Fields
		static string _id, _username;
		static IPAddress _ipAddress;
		#endregion

		#region Properties
		public static IPAddress IPAddress
		{
			get => _ipAddress ?? (_ipAddress = IPAddress.Parse(CrossSettings.Current.GetValueOrDefault(nameof(IPAddress), "0.0.0.0")));
			set
			{
				_ipAddress = value;
				CrossSettings.Current.AddOrUpdateValue(nameof(IPAddress), value.ToString());
			}
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
		#endregion

		#region Methods
		static string GetSetting(ref string backingStore, string defaultValue = "", [CallerMemberName]string propertyName = "") =>
			backingStore ?? (backingStore = CrossSettings.Current.GetValueOrDefault(propertyName, defaultValue));

		static void SetSetting(ref string backingStore, string value, [CallerMemberName]string propertyName = "")
		{
			if (EqualityComparer<string>.Default.Equals(backingStore, value))
				return;

			backingStore = value;

			CrossSettings.Current.AddOrUpdateValue(propertyName, value);
		}
		#endregion
	}
}
