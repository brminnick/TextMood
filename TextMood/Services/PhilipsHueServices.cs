using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;

using Refit;

using Xamarin.Essentials;
using Xamarin.Forms;

namespace TextMood
{
    abstract class PhilipsHueServices : BaseApiService
    {
        #region Constant Fields
        static readonly Lazy<IPhilipsHueApi> _philipsHueApiClientHolder = new Lazy<IPhilipsHueApi>(() => RestService.For<IPhilipsHueApi>("https://www.meethue.com/api"));
        #endregion

        #region Fields
        static IPhilipsHueBridgeApi _philipsHueBridgeApiClient;
        #endregion

        #region Constructors
        static PhilipsHueServices()
        {
            InitializePhilipsHueBridgeApiClient(PhilipsHueBridgeSettings.IPAddress);
            PhilipsHueBridgeSettings.IPAddressChanged += HandleIPAddressChanged;
        }
        #endregion

        #region Properties
        static IPhilipsHueApi PhilipsHueApiClient => _philipsHueApiClientHolder.Value;
        #endregion

        #region Methods
        public static async Task<int> GetNumberOfLights(string philipsHueBridgeUsername)
        {
            var isBridgeReachable = await IsBridgeReachable().ConfigureAwait(false);

            if (!isBridgeReachable)
                throw new Exception(GetBridgeNotFoundErrorMessage());

            var lightsResponseJObject = await ExecutePollyHttpFunction(() => _philipsHueBridgeApiClient.GetNumberOfLights(philipsHueBridgeUsername)).ConfigureAwait(false);
            return lightsResponseJObject.Count;
        }

        public static async Task<List<PhilipsHueUsernameDiscoveryModel>> AutoDetectUsername()
        {
            var isBridgeReachable = await IsBridgeReachable().ConfigureAwait(false);

            if (!isBridgeReachable)
                throw new Exception(GetBridgeNotFoundErrorMessage());

            var device = new PhilipsHueDeviceTypeModel { DeviceType = string.Empty };
            return await ExecutePollyHttpFunction(() => _philipsHueBridgeApiClient.AutoDetectUsername(device)).ConfigureAwait(false);
        }

        public static Task<List<PhilipsHueBridgeDiscoveryModel>> AutoDetectBridges() => ExecutePollyHttpFunction(PhilipsHueApiClient.AutoDetectBridges);

        public static async Task UpdateLightBulbColor(string philipsHueBridgeUsername, int hue)
        {
            var isBridgeReachable = await IsBridgeReachable().ConfigureAwait(false);

            if (!isBridgeReachable)
                throw new Exception(GetBridgeNotFoundErrorMessage());

            var hueRequest = new LightModel
            {
                On = true,
                Hue = hue,
                Saturation = hue >= 0 ? 255 : 0,
                Brightness = 255
            };

            var numberOfLights = await GetNumberOfLights(philipsHueBridgeUsername).ConfigureAwait(false);

            var lightAPIPutList = new List<Task>();
            for (int lightNumber = 1; lightNumber <= numberOfLights; lightNumber++)
                lightAPIPutList.Add(_philipsHueBridgeApiClient.UpdateLightBulbColor(hueRequest, philipsHueBridgeUsername, lightNumber));

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

        static async ValueTask<bool> IsBridgeReachable()
        {
            if (Connectivity.NetworkAccess is NetworkAccess.None)
                return false;

            try
            {
                var httpResult = await ExecutePollyHttpFunction(_philipsHueBridgeApiClient.GetPhilipsHueBridgeResponse, 1).ConfigureAwait(false);
                return httpResult.IsSuccessStatusCode;
            }
            catch
            {
                return false;
            }
        }

        static void HandleIPAddressChanged(object sender, IPAddress ipAddress) => InitializePhilipsHueBridgeApiClient(ipAddress);

        static void InitializePhilipsHueBridgeApiClient(string bridgeUrl) => _philipsHueBridgeApiClient = RestService.For<IPhilipsHueBridgeApi>(bridgeUrl);

        static void InitializePhilipsHueBridgeApiClient(IPAddress bridgeIPAddress) => InitializePhilipsHueBridgeApiClient($"http://{bridgeIPAddress.ToString()}");
        #endregion
    }
}