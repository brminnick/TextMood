using System;

using Plugin.Settings;

namespace TextMood
{
    public static class PhilipsHueBridgeSettings
    {
		#region Properties
        public static string IPAddress
        {
            get => CrossSettings.Current.GetValueOrDefault(nameof(IPAddress), string.Empty);
            set => CrossSettings.Current.AddOrUpdateValue(nameof(IPAddress), value);
        }

        public static string Id
        {
            get => CrossSettings.Current.GetValueOrDefault(nameof(Id), string.Empty);
            set => CrossSettings.Current.AddOrUpdateValue(nameof(Id), value);
        }

        public static string Username
        {
            get => CrossSettings.Current.GetValueOrDefault(nameof(Username), string.Empty);
            set => CrossSettings.Current.AddOrUpdateValue(nameof(Username), value);
        }
        #endregion
    }
}
