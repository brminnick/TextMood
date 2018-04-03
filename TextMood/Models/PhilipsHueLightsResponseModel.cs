using System;
using System.Collections.Generic;

using Newtonsoft.Json;

namespace TextMood
{
	public class PhilipsHueLightsResponseModel
	{
		[JsonProperty("state")]
		public State State { get; set; }

		[JsonProperty("swupdate")]
		public Swupdate Swupdate { get; set; }

		[JsonProperty("type")]
		public string Type { get; set; }

		[JsonProperty("name")]
		public string Name { get; set; }

		[JsonProperty("modelid")]
		public string Modelid { get; set; }

		[JsonProperty("manufacturername")]
		public string Manufacturername { get; set; }

		[JsonProperty("productname")]
		public string Productname { get; set; }

		[JsonProperty("capabilities")]
		public Capabilities Capabilities { get; set; }

		[JsonProperty("config")]
		public Config Config { get; set; }

		[JsonProperty("uniqueid")]
		public string Uniqueid { get; set; }

		[JsonProperty("swversion")]
		public string Swversion { get; set; }

		[JsonProperty("swconfigid")]
		public string Swconfigid { get; set; }

		[JsonProperty("productid")]
		public string Productid { get; set; }
	}

	public class Capabilities
	{
		[JsonProperty("certified")]
		public bool Certified { get; set; }

		[JsonProperty("control")]
		public Control Control { get; set; }

		[JsonProperty("streaming")]
		public Streaming Streaming { get; set; }
	}

	public class Control
	{
		[JsonProperty("mindimlevel")]
		public long Mindimlevel { get; set; }

		[JsonProperty("maxlumen")]
		public long Maxlumen { get; set; }

		[JsonProperty("colorgamuttype")]
		public string Colorgamuttype { get; set; }

		[JsonProperty("colorgamut")]
		public List<List<double>> Colorgamut { get; set; }

		[JsonProperty("ct")]
		public Ct Ct { get; set; }
	}

	public class Ct
	{
		[JsonProperty("min")]
		public long Min { get; set; }

		[JsonProperty("max")]
		public long Max { get; set; }
	}

	public class Streaming
	{
		[JsonProperty("renderer")]
		public bool Renderer { get; set; }

		[JsonProperty("proxy")]
		public bool Proxy { get; set; }
	}

	public class Config
	{
		[JsonProperty("archetype")]
		public string Archetype { get; set; }

		[JsonProperty("function")]
		public string Function { get; set; }

		[JsonProperty("direction")]
		public string Direction { get; set; }
	}

	public class State
	{
		[JsonProperty("on")]
		public bool On { get; set; }

		[JsonProperty("bri")]
		public long Brightness { get; set; }

		[JsonProperty("hue")]
		public long Hue { get; set; }

		[JsonProperty("sat")]
		public long Saturation { get; set; }

		[JsonProperty("effect")]
		public string Effect { get; set; }

		[JsonProperty("xy")]
		public List<double> CIEColorCordinates { get; set; }

		[JsonProperty("ct")]
		public long ColorTemperature { get; set; }

		[JsonProperty("alert")]
		public string Alert { get; set; } = "none";

		[JsonProperty("colormode")]
		public string ColorMode { get; set; }

		[JsonProperty("mode")]
		public string Mode { get; set; }

		[JsonProperty("reachable")]
		public bool IsReachable { get; set; }
	}

	public class LightModel
	{
		[JsonProperty("on")]
		public bool On { get; set; }

		[JsonProperty("bri")]
		public long Brightness { get; set; }

		[JsonProperty("hue")]
		public long Hue { get; set; }

		[JsonProperty("sat")]
		public long Saturation { get; set; }
	}

	public class Swupdate
	{
		[JsonProperty("state")]
		public string State { get; set; }

		[JsonProperty("lastinstall")]
		public DateTimeOffset Lastinstall { get; set; }
	}
}
