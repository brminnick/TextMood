using Newtonsoft.Json;

namespace TextMood
{
    public class PhilipsHueDeviceTypeModel
    {
		[JsonProperty("devicetype")]
        public string DeviceType { get; set; }
    }
}
