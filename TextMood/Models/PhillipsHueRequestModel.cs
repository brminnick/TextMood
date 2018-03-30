using System;

using Newtonsoft.Json;

namespace TextMood
{
    public class PhillipsHueRequestModel
    {
		[JsonProperty("on")]
        public bool On { get; set; }

        [JsonProperty("sat")]
        public long Saturation { get; set; }

        [JsonProperty("bri")]
        public long Brightness { get; set; }

        [JsonProperty("hue")]
        public long Hue { get; set; }
    }
}
