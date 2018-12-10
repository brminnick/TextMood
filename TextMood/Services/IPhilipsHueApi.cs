using System.Collections.Generic;
using System.Threading.Tasks;
using Refit;

namespace TextMood
{
    public interface IPhilipsHueApi
    {
        [Get("/nupnp")]
        Task<List<PhilipsHueBridgeDiscoveryModel>> AutoDetectBridges();
    }
}
