using Newtonsoft.Json;

namespace TextMood
{
    public class PhillipsHueDeviceTypeModel
    {
		[JsonProperty("devicetype")]
        public string DeviceType { get; set; }
    }
}
