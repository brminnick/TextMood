using Newtonsoft.Json;

namespace TextMood
{
    public class PhilipsHueDeviceTypeModel
    {
        public PhilipsHueDeviceTypeModel(string devicetype) => DeviceType = devicetype;

        [JsonProperty("devicetype")]
        public string DeviceType { get; set; }
    }
}
