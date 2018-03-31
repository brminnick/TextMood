using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;

using Newtonsoft.Json.Linq;

using Plugin.Settings;
using Plugin.Connectivity;

using Xamarin.Forms;

namespace TextMood
{
	abstract class PhillipsHueBridgeServices : BaseHttpClientService
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

		public static string PhillipsBridgeUserName
		{
			get => CrossSettings.Current.GetValueOrDefault(nameof(PhillipsBridgeUserName), string.Empty);
			set => CrossSettings.Current.AddOrUpdateValue(nameof(PhillipsBridgeUserName), value);
		}
		#endregion

		#region Methods
		public static async ValueTask<int> GetNumberOfLights()
		{
			var isBridgeReachable = await IsBridgeReachable().ConfigureAwait(false);
			if (!isBridgeReachable)
				throw new Exception(GetBridgeNotFoundErrorMessage());

			var lightsResponseJObject = await GetObjectFromAPI<JObject>($"http://{PhillipsHueBridgeIPAddress}/api/{PhillipsBridgeUserName}/lights").ConfigureAwait(false);
			return lightsResponseJObject.Count;
		}

		public static async ValueTask<List<PhillipsHueUserNameDiscoveryModel>> AutoDetectUserName()
		{
			var isBridgeReachable = await IsBridgeReachable().ConfigureAwait(false);
			if (!isBridgeReachable)
				throw new Exception(GetBridgeNotFoundErrorMessage());

			var deviceType = new PhillipsHueDeviceTypeModel { Devicetype = string.Empty };
			return await PostObjectToAPI<List<PhillipsHueUserNameDiscoveryModel>, PhillipsHueDeviceTypeModel>($"http://{PhillipsHueBridgeIPAddress}/api", deviceType).ConfigureAwait(false);
		}

		public static Task<List<PhillipsHueBridgeDiscoveryModel>> AutoDetectBridges() =>
			GetObjectFromAPI<List<PhillipsHueBridgeDiscoveryModel>>("https://www.meethue.com/api/nupnp");

		public static async Task UpdateLightBulbColor(int hue)
		{
			var isBridgeReachable = await IsBridgeReachable().ConfigureAwait(false);
			if (!isBridgeReachable)
				throw new Exception(GetBridgeNotFoundErrorMessage());

			var hueRequest = new State
			{
				On = true,
				Hue = hue,
				Saturation = hue >= 0 ? 255 : 0,
				Brightness = 255
			};

			var numberOfLights = await GetNumberOfLights().ConfigureAwait(false);

			var lightAPIPutList = new List<Task>();
			for (int lightNumber = 1; lightNumber <= numberOfLights; lightNumber++)
				lightAPIPutList.Add(PutObjectToAPI($"http://{PhillipsHueBridgeIPAddress}/api/{PhillipsBridgeUserName}/lights/{lightNumber}/state", hueRequest));

			await Task.WhenAll(lightAPIPutList).ConfigureAwait(false);
		}

		static async ValueTask<bool> IsBridgeReachable()
		{
			try
			{
				return CrossConnectivity.Current.IsConnected
										&& await CrossConnectivity.Current.IsRemoteReachable(PhillipsHueBridgeIPAddress).ConfigureAwait(false);
			}
			catch (ArgumentNullException)
			{
				return false;
			}
		}

		static string GetBridgeNotFoundErrorMessage()
		{
			const string bridgeNotFoundError = "Bridge Not Found.";

			switch (Device.Idiom)
			{
				case TargetIdiom.Desktop:
					return $"{bridgeNotFoundError} Ensure this computer and the Phillips Bridge are connected to the same local network";
				case TargetIdiom.Phone:
					return $"{bridgeNotFoundError} Ensure this phone and the Phillips Bridge are connected to the same local network";
				case TargetIdiom.Tablet:
					return $"{bridgeNotFoundError} Ensure this tablet and the Phillips Bridge are connected to the same local network";
				case TargetIdiom.TV:
					return $"{bridgeNotFoundError} Ensure this TV and the Phillips Bridge are connected to the same local network";
				default:
					return $"{bridgeNotFoundError} Ensure this app and the Phillips Bridge are connected to the same local network";
			}
		}
		#endregion
	}
}