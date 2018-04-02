using System;
using Plugin.Settings;

namespace TextMood
{
    public static class PhillipsHueBridgeSettings
    {
		#region Properties
        public static string PhillipsHueBridgeIPAddress
        {
            get => CrossSettings.Current.GetValueOrDefault(nameof(PhillipsHueBridgeIPAddress), string.Empty);
            set => CrossSettings.Current.AddOrUpdateValue(nameof(PhillipsHueBridgeIPAddress), value);
        }

        public static string PhillipsHueBridgeID
        {
            get => CrossSettings.Current.GetValueOrDefault(nameof(PhillipsHueBridgeID), string.Empty);
            set => CrossSettings.Current.AddOrUpdateValue(nameof(PhillipsHueBridgeID), value);
        }

        public static string PhillipsBridgeUsername
        {
            get => CrossSettings.Current.GetValueOrDefault(nameof(PhillipsBridgeUsername), string.Empty);
            set => CrossSettings.Current.AddOrUpdateValue(nameof(PhillipsBridgeUsername), value);
        }
        #endregion
    }
}
