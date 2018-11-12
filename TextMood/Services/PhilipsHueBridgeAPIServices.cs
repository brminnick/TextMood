using System;
using System.Collections.Generic;
using System.Threading.Tasks;

using Newtonsoft.Json.Linq;

using Xamarin.Forms;
using Xamarin.Essentials;

namespace TextMood
{
    abstract class PhilipsHueBridgeAPIServices : BaseHttpClientService
    {
        #region Methods
        public static async Task<int> GetNumberOfLights(string philipsHueBridgeIPAddress, string philipsHueBridgeUsername)
        {
            var isBridgeReachable = await IsBridgeReachable(philipsHueBridgeIPAddress).ConfigureAwait(false);

            if (!isBridgeReachable)
                throw new Exception(GetBridgeNotFoundErrorMessage());

            var lightsResponseJObject = await GetObjectFromAPI<JObject>($"http://{philipsHueBridgeIPAddress}/api/{philipsHueBridgeUsername}/lights").ConfigureAwait(false);
            return lightsResponseJObject.Count;
        }

        public static async Task<List<PhilipsHueUsernameDiscoveryModel>> AutoDetectUsername(string philipsHueBridgeIPAddress)
        {
            var isBridgeReachable = await IsBridgeReachable(philipsHueBridgeIPAddress).ConfigureAwait(false);

            if (!isBridgeReachable)
                throw new Exception(GetBridgeNotFoundErrorMessage());

            var deviceType = new PhilipsHueDeviceTypeModel { DeviceType = string.Empty };
            return await PostObjectToAPI<List<PhilipsHueUsernameDiscoveryModel>, PhilipsHueDeviceTypeModel>($"http://{philipsHueBridgeIPAddress}/api", deviceType).ConfigureAwait(false);
        }

        public static Task<List<PhilipsHueBridgeDiscoveryModel>> AutoDetectBridges() =>
            GetObjectFromAPI<List<PhilipsHueBridgeDiscoveryModel>>("https://www.meethue.com/api/nupnp");

        public static async Task UpdateLightBulbColor(string philipsHueBridgeIPAddress, string philipsHueBridgeUsername, int hue)
        {
            var isBridgeReachable = await IsBridgeReachable(philipsHueBridgeIPAddress).ConfigureAwait(false);

            if (!isBridgeReachable)
                throw new Exception(GetBridgeNotFoundErrorMessage());

            var hueRequest = new LightModel
            {
                On = true,
                Hue = hue,
                Saturation = hue >= 0 ? 255 : 0,
                Brightness = 255
            };

            var numberOfLights = await GetNumberOfLights(philipsHueBridgeIPAddress, philipsHueBridgeUsername).ConfigureAwait(false);

            var lightAPIPutList = new List<Task>();
            for (int lightNumber = 1; lightNumber <= numberOfLights; lightNumber++)
                lightAPIPutList.Add(PutObjectToAPI($"http://{philipsHueBridgeIPAddress}/api/{philipsHueBridgeUsername}/lights/{lightNumber}/state", hueRequest));

            await Task.WhenAll(lightAPIPutList).ConfigureAwait(false);
        }

        static string GetBridgeNotFoundErrorMessage()
        {
            const string bridgeNotFoundError = "Bridge Not Found.";

            switch (Device.Idiom)
            {
                case TargetIdiom.Desktop:
                    return $"{bridgeNotFoundError} Ensure this computer and the Philips Bridge are connected to the same local network.";
                case TargetIdiom.Phone:
                    return $"{bridgeNotFoundError} Ensure this phone and the Philips Bridge are connected to the same local network.";
                case TargetIdiom.Tablet:
                    return $"{bridgeNotFoundError} Ensure this tablet and the Philips Bridge are connected to the same local network.";
                case TargetIdiom.TV:
                    return $"{bridgeNotFoundError} Ensure this TV and the Philips Bridge are connected to the same local network.";
                default:
                    return $"{bridgeNotFoundError} Ensure this app and the Philips Bridge are connected to the same local network.";
            }
        }

        static async ValueTask<bool> IsBridgeReachable(string philipsHueBridgeIPAddress)
        {
            if (Connectivity.NetworkAccess is NetworkAccess.None)
                return false;

            try
            {
                var httpResult = await GetObjectFromAPI($"http://{philipsHueBridgeIPAddress}").ConfigureAwait(false);
                return httpResult.IsSuccessStatusCode;
            }
            catch
            {
                return false;
            }
        }
        #endregion
    }
}