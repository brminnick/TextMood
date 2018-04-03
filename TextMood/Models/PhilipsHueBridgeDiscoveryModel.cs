using Newtonsoft.Json;

namespace TextMood
{
    public class PhilipsHueBridgeDiscoveryModel
    {
		[JsonProperty("id")]
        public string Id { get; set; }

        [JsonProperty("internalipaddress")]
		public string InternalIPAddress { get; set; }
    }
}
