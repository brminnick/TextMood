using Newtonsoft.Json;

namespace TextMood
{
    public class PhillipsHueUsernameDiscoveryModel
    {
		[JsonProperty("error")]
		public Error Error { get; set; }

		[JsonProperty("success")]
        public Success Success { get; set; }
    }

	public class Error
    {
        [JsonProperty("type")]
        public long Type { get; set; }

        [JsonProperty("address")]
        public string Address { get; set; }

        [JsonProperty("description")]
        public string Description { get; set; }
    }

	public class Success
    {
        [JsonProperty("username")]
		public string Username { get; set; }
    }
}
