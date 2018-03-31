using Newtonsoft.Json;

namespace TextMood
{
    public class PhillipsHueBridgeDiscoveryModel
    {
		[JsonProperty("id")]
        public string Id { get; set; }

        [JsonProperty("internalipaddress")]
		public string InternalIPAddress { get; set; }
    }
}
