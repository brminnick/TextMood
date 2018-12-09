using System;
using System.Collections.Generic;
using System.Threading.Tasks;

using Refit;
using Xamarin.Forms;
using Xamarin.Essentials;

namespace TextMood
{
    abstract class PhilipsHueBridgeAPIServices : BaseApiService
    {
        #region Constant Fields
        readonly static Lazy<IPhilipsHueApi> _philipsHueApiClientHolder = new Lazy<IPhilipsHueApi>(() => RestService.For<IPhilipsHueApi>(""));
        #endregion

        #region Properties
        static IPhilipsHueApi PhilipsHueApiClient => _philipsHueApiClientHolder.Value;
        #endregion

        #region Methods
        public static async Task<int> GetNumberOfLights(string philipsHueBridgeIPAddress, string philipsHueBridgeUsername)
        {
            var isBridgeReachable = await IsBridgeReachable(philipsHueBridgeIPAddress).ConfigureAwait(false);

            if (!isBridgeReachable)
                throw new Exception(GetBridgeNotFoundErrorMessage());

            var lightsResponseJObject = await ExecutePollyHttpFunction(() => PhilipsHueApiClient.GetNumberOfLights(philipsHueBridgeIPAddress, philipsHueBridgeUsername)).ConfigureAwait(false);
            return lightsResponseJObject.Count;
        }

        public static async Task<List<PhilipsHueUsernameDiscoveryModel>> AutoDetectUsername(string philipsHueBridgeIPAddress)
        {
            var isBridgeReachable = await IsBridgeReachable(philipsHueBridgeIPAddress).ConfigureAwait(false);

            if (!isBridgeReachable)
                throw new Exception(GetBridgeNotFoundErrorMessage());

            var device = new PhilipsHueDeviceTypeModel { DeviceType = string.Empty };
            return await ExecutePollyHttpFunction(() => PhilipsHueApiClient.AutoDetectUsername(device, philipsHueBridgeIPAddress)).ConfigureAwait(false);
        }

        public static Task<List<PhilipsHueBridgeDiscoveryModel>> AutoDetectBridges() => ExecutePollyHttpFunction(AutoDetectBridges);

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
                lightAPIPutList.Add(PhilipsHueApiClient.UpdateLightBulbColor(hueRequest, philipsHueBridgeIPAddress, philipsHueBridgeUsername, lightNumber));

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
                var httpResult = await ExecutePollyHttpFunction(PhilipsHueApiClient.GetPhilipsHueBridge).ConfigureAwait(false);
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