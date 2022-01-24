using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace TextMood
{
    public class PhilipsHueServices : BaseApiService
    {
        readonly IPhilipsHueApi _philipsHueApiClient;
        readonly PhilipsHueBridgeSettingsService _philipsHueBridgeSettingsService;

        IPhilipsHueBridgeApi _philipsHueBridgeApiClient;

        public PhilipsHueServices(IPhilipsHueApi philipsHueApi, PhilipsHueBridgeSettingsService philipsHueBridgeSettingsService)
        {
            _philipsHueApiClient = philipsHueApi;
            _philipsHueBridgeSettingsService = philipsHueBridgeSettingsService;

            _philipsHueBridgeSettingsService.IPAddressChanged += HandleIPAddressChanged;
            _philipsHueBridgeApiClient = InitializePhilipsHueBridgeApiClient(philipsHueBridgeSettingsService.IPAddress);
        }

        public async Task<int> GetNumberOfLights(string philipsHueBridgeUsername)
        {
            var isBridgeReachable = await IsBridgeReachable().ConfigureAwait(false);

            if (!isBridgeReachable)
                throw new Exception(GetBridgeNotFoundErrorMessage());

            var lightsResponseJObject = await AttemptAndRetry(() => _philipsHueBridgeApiClient.GetNumberOfLights(philipsHueBridgeUsername), 1).ConfigureAwait(false);
            return lightsResponseJObject.Count;
        }

        public async Task<IReadOnlyList<PhilipsHueUsernameDiscoveryModel>> AutoDetectUsername()
        {
            var isBridgeReachable = await IsBridgeReachable().ConfigureAwait(false);

            if (!isBridgeReachable)
                throw new Exception(GetBridgeNotFoundErrorMessage());

            var device = new PhilipsHueDeviceTypeModel(string.Empty);
            return await AttemptAndRetry(() => _philipsHueBridgeApiClient.AutoDetectUsername(device), 1).ConfigureAwait(false);
        }

        public Task<IReadOnlyList<PhilipsHueBridgeDiscoveryModel>> AutoDetectBridges() => AttemptAndRetry(_philipsHueApiClient.AutoDetectBridges, 1);

        public async Task UpdateLightBulbColor(string philipsHueBridgeUsername, int hue)
        {
            var isBridgeReachable = await IsBridgeReachable().ConfigureAwait(false);

            if (!isBridgeReachable)
                throw new Exception(GetBridgeNotFoundErrorMessage());

            var hueRequest = new LightModel(true, 255, hue, hue >= 0 ? 255 : 0);

            var numberOfLights = await GetNumberOfLights(philipsHueBridgeUsername).ConfigureAwait(false);

            var lightAPIPutList = new List<Task>();
            for (int lightNumber = 1; lightNumber <= numberOfLights; lightNumber++)
                lightAPIPutList.Add(_philipsHueBridgeApiClient.UpdateLightBulbColor(hueRequest, philipsHueBridgeUsername, lightNumber));

            await Task.WhenAll(lightAPIPutList).ConfigureAwait(false);
        }

        string GetBridgeNotFoundErrorMessage()
        {
            const string bridgeNotFoundError = "Bridge Not Found.";

            return Device.Idiom switch
            {
                TargetIdiom.Desktop => $"{bridgeNotFoundError} Ensure this computer and the Philips Bridge are connected to the same local network.",
                TargetIdiom.Phone => $"{bridgeNotFoundError} Ensure this phone and the Philips Bridge are connected to the same local network.",
                TargetIdiom.Tablet => $"{bridgeNotFoundError} Ensure this tablet and the Philips Bridge are connected to the same local network.",
                TargetIdiom.TV => $"{bridgeNotFoundError} Ensure this TV and the Philips Bridge are connected to the same local network.",
                _ => $"{bridgeNotFoundError} Ensure this app and the Philips Bridge are connected to the same local network.",
            };
        }

        async Task<bool> IsBridgeReachable()
        {
            if (Connectivity.NetworkAccess is NetworkAccess.None)
                return false;

            try
            {
                var httpResult = await AttemptAndRetry(_philipsHueBridgeApiClient.GetPhilipsHueBridgeResponse, 1).ConfigureAwait(false);
                return httpResult.IsSuccessStatusCode;
            }
            catch
            {
                return false;
            }
        }

        void HandleIPAddressChanged(object sender, IPAddress ipAddress) => _philipsHueBridgeApiClient = InitializePhilipsHueBridgeApiClient(ipAddress);

        IPhilipsHueBridgeApi InitializePhilipsHueBridgeApiClient(string bridgeUrl) => RefitExtensions.For<IPhilipsHueBridgeApi>(bridgeUrl);

        IPhilipsHueBridgeApi InitializePhilipsHueBridgeApiClient(IPAddress bridgeIPAddress) => InitializePhilipsHueBridgeApiClient($"http://{bridgeIPAddress.ToString()}");
    }
}