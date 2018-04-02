using System;
using System.Collections.Generic;
using System.Threading.Tasks;

using Newtonsoft.Json.Linq;

using Plugin.Connectivity;

using Xamarin.Forms;

namespace TextMood
{
	abstract class PhillipsHueBridgeAPIServices : BaseHttpClientService
	{
		#region Methods
		public static async ValueTask<int> GetNumberOfLights(string phillipsHueBridgeIPAddress, string phillipsHueBridgeUsername)
		{
			var isBridgeReachable = await IsBridgeReachable(phillipsHueBridgeIPAddress).ConfigureAwait(false);
			if (!isBridgeReachable)
				throw new Exception(GetBridgeNotFoundErrorMessage());

			var lightsResponseJObject = await GetObjectFromAPI<JObject>($"http://{phillipsHueBridgeIPAddress}/api/{phillipsHueBridgeUsername}/lights").ConfigureAwait(false);
			return lightsResponseJObject.Count;
		}

		public static async ValueTask<List<PhillipsHueUsernameDiscoveryModel>> AutoDetectUsername(string phillipsHueBridgeIPAddress)
		{
			var isBridgeReachable = await IsBridgeReachable(phillipsHueBridgeIPAddress).ConfigureAwait(false);
			if (!isBridgeReachable)
				throw new Exception(GetBridgeNotFoundErrorMessage());

			var deviceType = new PhillipsHueDeviceTypeModel { DeviceType = string.Empty };
			return await PostObjectToAPI<List<PhillipsHueUsernameDiscoveryModel>, PhillipsHueDeviceTypeModel>($"http://{phillipsHueBridgeIPAddress}/api", deviceType).ConfigureAwait(false);
		}

		public static Task<List<PhillipsHueBridgeDiscoveryModel>> AutoDetectBridges() =>
			GetObjectFromAPI<List<PhillipsHueBridgeDiscoveryModel>>("https://www.meethue.com/api/nupnp");

		public static async Task UpdateLightBulbColor(string phillipsHueBridgeIPAddress, string phillipsHueBridgeUsername, int hue)
		{
			var isBridgeReachable = await IsBridgeReachable(phillipsHueBridgeIPAddress).ConfigureAwait(false);
			if (!isBridgeReachable)
				throw new Exception(GetBridgeNotFoundErrorMessage());

			var hueRequest = new LightModel
			{
				On = true,
				Hue = hue,
				Saturation = hue >= 0 ? 255 : 0,
				Brightness = 255
			};

			var numberOfLights = await GetNumberOfLights(phillipsHueBridgeIPAddress, phillipsHueBridgeUsername).ConfigureAwait(false);

			var lightAPIPutList = new List<Task>();
			for (int lightNumber = 1; lightNumber <= numberOfLights; lightNumber++)
				lightAPIPutList.Add(PutObjectToAPI($"http://{phillipsHueBridgeIPAddress}/api/{phillipsHueBridgeUsername}/lights/{lightNumber}/state", hueRequest));

			await Task.WhenAll(lightAPIPutList).ConfigureAwait(false);
		}

		static async ValueTask<bool> IsBridgeReachable(string phillipsHueBridgeIPAddress)
		{
			try
			{
				return CrossConnectivity.Current.IsConnected
										&& await CrossConnectivity.Current.IsRemoteReachable(phillipsHueBridgeIPAddress).ConfigureAwait(false);
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