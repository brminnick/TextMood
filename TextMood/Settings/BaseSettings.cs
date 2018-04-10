using System.Collections.Generic;
using System.Net;
using System.Runtime.CompilerServices;

using Plugin.Settings;
using System;

namespace TextMood
{
	abstract class BaseSettings
	{
		#region Methods
		protected static string GetSetting(ref string backingStore, string defaultValue = "", [CallerMemberName]string propertyName = "") =>
			backingStore ?? (backingStore = CrossSettings.Current.GetValueOrDefault(propertyName, defaultValue));

		protected static bool GetSetting(ref bool backingStore, bool defaultValue = default, [CallerMemberName]string propertyName = "") =>
		    backingStore.Equals(default(bool)) ? (backingStore = CrossSettings.Current.GetValueOrDefault(propertyName, defaultValue)) : backingStore;

		protected static int GetSetting(ref int backingStore, int defaultValue = default, [CallerMemberName]string propertyName = "") =>
			backingStore.Equals(default(int)) ? (backingStore = CrossSettings.Current.GetValueOrDefault(propertyName, defaultValue)) : backingStore;

		protected static DateTimeOffset GetSetting(ref DateTimeOffset backingStore, DateTimeOffset defaultValue = default, [CallerMemberName]string propertyName = "") =>
			backingStore.Equals(default(DateTimeOffset)) ? (backingStore = new DateTimeOffset(CrossSettings.Current.GetValueOrDefault(propertyName, defaultValue.UtcDateTime))) : backingStore;

		protected static IPAddress GetSetting(ref IPAddress backingStore, [CallerMemberName]string propertyName = "") =>
			backingStore ?? (backingStore = IPAddress.Parse(CrossSettings.Current.GetValueOrDefault(propertyName, "0.0.0.0")));

		protected static void SetSetting(ref string backingStore, string value, [CallerMemberName]string propertyName = "")
		{
			if (EqualityComparer<string>.Default.Equals(backingStore, value))
				return;

			backingStore = value;

			CrossSettings.Current.AddOrUpdateValue(propertyName, value);
		}

		protected static void SetSetting(ref bool backingStore, bool value, [CallerMemberName]string propertyName = "")
        {
			if (EqualityComparer<bool>.Default.Equals(backingStore, value))
                return;

            backingStore = value;

            CrossSettings.Current.AddOrUpdateValue(propertyName, value);
        }

		protected static void SetSetting(ref int backingStore, int value, [CallerMemberName]string propertyName = "")
        {
			if (EqualityComparer<int>.Default.Equals(backingStore, value))
                return;

            backingStore = value;

            CrossSettings.Current.AddOrUpdateValue(propertyName, value);
        }

		protected static void SetSetting(ref DateTimeOffset backingStore, DateTimeOffset value, [CallerMemberName]string propertyName = "")
		{
			if (EqualityComparer<DateTimeOffset>.Default.Equals(backingStore, value))
				return;

			backingStore = value;

			CrossSettings.Current.AddOrUpdateValue(propertyName, value.UtcDateTime);
		}

		protected static void SetSetting(ref IPAddress backingStore, IPAddress value, [CallerMemberName]string propertyName = "")
		{
			if (EqualityComparer<IPAddress>.Default.Equals(backingStore, value))
				return;

			backingStore = value;

			CrossSettings.Current.AddOrUpdateValue(propertyName, value.ToString());
		}
		#endregion
	}
}
