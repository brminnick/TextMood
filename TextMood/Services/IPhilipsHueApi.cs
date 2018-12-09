using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using Refit;

namespace TextMood
{
    public interface IPhilipsHueApi
    {
        [Get("http://{philipsHueBridgeIPAddress}/api/{philipsHueBridgeUsername}/lights")]
        Task<JObject> GetNumberOfLights(string philipsHueBridgeIPAddress, string philipsHueBridgeUsername);

        [Post("http://{philipsHueBridgeIPAddress}/api")]
        Task<List<PhilipsHueUsernameDiscoveryModel>> AutoDetectUsername([Body]PhilipsHueDeviceTypeModel device, string philipsHueBridgeIPAddress);

        [Get("https://www.meethue.com/api/nupnp")]
        Task<List<PhilipsHueBridgeDiscoveryModel>> AutoDetectBridges();

        [Put("http://{philipsHueBridgeIPAddress}/api/{philipsHueBridgeUsername}/lights/{lightNumber}/state")]
        Task UpdateLightBulbColor([Body]LightModel light, string philipsHueBridgeIPAddress, string philipsHueBridgeUsername, int lightNumber);

        [Get("http://{philipsHueBridgeIPAddress}")]
        Task<HttpResponseMessage> GetPhilipsHueBridge();
    }
}
