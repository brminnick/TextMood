using Newtonsoft.Json;

namespace TextMood
{
    public class PhilipsHueBridgeDiscoveryModel
    {
        public PhilipsHueBridgeDiscoveryModel(string id, string internalipaddress) =>
            (Id, InternalIPAddress) = (id, internalipaddress);

        [JsonProperty("id")]
        public string Id { get; }

        [JsonProperty("internalipaddress")]
        public string InternalIPAddress { get; }
    }
}
