using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using Refit;

namespace TextMood
{
    public interface IPhilipsHueBridgeApi
    {
        [Get("/api/{philipsHueBridgeUsername}/lights")]
        Task<JObject> GetNumberOfLights(string philipsHueBridgeUsername);

        [Post("/api")]
        Task<IReadOnlyList<PhilipsHueUsernameDiscoveryModel>> AutoDetectUsername([Body]PhilipsHueDeviceTypeModel device);

        [Put("/api/{philipsHueBridgeUsername}/lights/{lightNumber}/state")]
        Task UpdateLightBulbColor([Body]LightModel light, string philipsHueBridgeUsername, int lightNumber);

        [Get("/")]
        Task<HttpResponseMessage> GetPhilipsHueBridgeResponse();
    }
}
