using System;
using System.Collections.Generic;
using System.Net;
using System.Runtime.CompilerServices;

using Xamarin.Essentials;

namespace TextMood
{
	abstract class BaseSettings
	{
		#region Methods
		protected static string GetSetting(ref string backingStore, string defaultValue = "", [CallerMemberName]string propertyName = "") =>
			backingStore ?? (backingStore = Preferences.Get(propertyName, defaultValue));

		protected static bool GetSetting(ref bool backingStore, bool defaultValue = default, [CallerMemberName]string propertyName = "") =>
		    backingStore is default(bool) ? (backingStore = Preferences.Get(propertyName, defaultValue)) : backingStore;

		protected static int GetSetting(ref int backingStore, int defaultValue = default, [CallerMemberName]string propertyName = "") =>
			backingStore is default(int) ? (backingStore = Preferences.Get(propertyName, defaultValue)) : backingStore;

		protected static DateTimeOffset GetSetting(ref DateTimeOffset backingStore, DateTimeOffset defaultValue = default, [CallerMemberName]string propertyName = "") =>
			backingStore.Equals(default(DateTimeOffset)) ? (backingStore = new DateTimeOffset(Preferences.Get(propertyName, defaultValue.UtcDateTime))) : backingStore;

		protected static IPAddress GetSetting(ref IPAddress backingStore, [CallerMemberName]string propertyName = "") =>
			backingStore ?? (backingStore = IPAddress.Parse(Preferences.Get(propertyName, "0.0.0.0")));

		protected static void SetSetting(ref string backingStore, string value, Action onSettingChanged = null, [CallerMemberName]string propertyName = "")
        {
			if (EqualityComparer<string>.Default.Equals(backingStore, value))
				return;

			backingStore = value;

			Preferences.Set(propertyName, value);

            onSettingChanged?.Invoke();
        }

		protected static void SetSetting(ref bool backingStore, bool value, Action onSettingChanged = null, [CallerMemberName]string propertyName = "")
        {
			if (EqualityComparer<bool>.Default.Equals(backingStore, value))
                return;

            backingStore = value;

            Preferences.Set(propertyName, value);

            onSettingChanged?.Invoke();
        }

		protected static void SetSetting(ref int backingStore, int value, Action onSettingChanged = null, [CallerMemberName]string propertyName = "")
        {
			if (EqualityComparer<int>.Default.Equals(backingStore, value))
                return;

            backingStore = value;

            Preferences.Set(propertyName, value);

            onSettingChanged?.Invoke();
        }

		protected static void SetSetting(ref DateTimeOffset backingStore, DateTimeOffset value, Action onSettingChanged = null, [CallerMemberName]string propertyName = "")
        {
			if (EqualityComparer<DateTimeOffset>.Default.Equals(backingStore, value))
				return;

			backingStore = value;

			Preferences.Set(propertyName, value.UtcDateTime);

            onSettingChanged?.Invoke();
        }

		protected static void SetSetting(ref IPAddress backingStore, IPAddress value, Action onSettingChanged = null, [CallerMemberName]string propertyName = "")
        {
			if (EqualityComparer<IPAddress>.Default.Equals(backingStore, value))
				return;

			backingStore = value;

			Preferences.Set(propertyName, value.ToString());

            onSettingChanged?.Invoke();
        }
		#endregion
	}
}
