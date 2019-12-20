using Newtonsoft.Json;

namespace TextMood
{
    public class PhilipsHueUsernameDiscoveryModel
    {
        public PhilipsHueUsernameDiscoveryModel(Error error, Success success) =>
            (Error, Success) = (error, success);

        [JsonProperty("error")]
        public Error Error { get; }

        [JsonProperty("success")]
        public Success Success { get; }
    }

    public class Error
    {
        public Error(long type, string address, string description) =>
            (Type, Address, Description) = (type, address, description);

        [JsonProperty("type")]
        public long Type { get; }

        [JsonProperty("address")]
        public string Address { get; }

        [JsonProperty("description")]
        public string Description { get; }
    }

    public class Success
    {
        public Success(string username) => Username = username;

        [JsonProperty("username")]
        public string Username { get; }
    }
}
