using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;

namespace TextMood
{
    public class PhilipsHueLightsResponseModel
    {
        public PhilipsHueLightsResponseModel(State state, Swupdate swupdate, string type, string name, string modelid, string manufacturername, string productname, Capabilities capabilities, Config config, string uniqueid, string swversion, string swconfigid, string productid) =>
            (State, Swupdate, Type, Name, Modelid, Manufacturername, Productname, Capabilities, Config, Uniqueid, Swversion, Swconfigid, Productid) = (state, swupdate, type, name, modelid, manufacturername, productname, capabilities, config, uniqueid, swversion, swconfigid, productid);

        [JsonProperty("state")]
        public State State { get; }

        [JsonProperty("swupdate")]
        public Swupdate Swupdate { get; }

        [JsonProperty("type")]
        public string Type { get; }

        [JsonProperty("name")]
        public string Name { get; }

        [JsonProperty("modelid")]
        public string Modelid { get; }

        [JsonProperty("manufacturername")]
        public string Manufacturername { get; }

        [JsonProperty("productname")]
        public string Productname { get; }

        [JsonProperty("capabilities")]
        public Capabilities Capabilities { get; }

        [JsonProperty("config")]
        public Config Config { get; }

        [JsonProperty("uniqueid")]
        public string Uniqueid { get; }

        [JsonProperty("swversion")]
        public string Swversion { get; }

        [JsonProperty("swconfigid")]
        public string Swconfigid { get; }

        [JsonProperty("productid")]
        public string Productid { get; }
    }

    public class Capabilities
    {
        public Capabilities(bool certified, Control control, Streaming streaming) =>
            (Certified, Control, Streaming) = (certified, control, streaming);

        [JsonProperty("certified")]
        public bool Certified { get; }

        [JsonProperty("control")]
        public Control Control { get; }

        [JsonProperty("streaming")]
        public Streaming Streaming { get; }
    }

    public class Control
    {
        public Control(long mindimlevel, long maxlumen, string colorgamuttype, List<List<double>> colorgamut, Ct ct) =>
            (Mindimlevel, Maxlumen, Colorgamuttype, Colorgamut, Ct) = (mindimlevel, maxlumen, colorgamuttype, colorgamut, ct);

        [JsonProperty("mindimlevel")]
        public long Mindimlevel { get; }

        [JsonProperty("maxlumen")]
        public long Maxlumen { get; }

        [JsonProperty("colorgamuttype")]
        public string Colorgamuttype { get; }

        [JsonProperty("colorgamut")]
        public List<List<double>> Colorgamut { get; }

        [JsonProperty("ct")]
        public Ct Ct { get; }
    }

    public class Ct
    {
        public Ct(long min, long max) => (Min, Max) = (min, max);

        [JsonProperty("min")]
        public long Min { get; }

        [JsonProperty("max")]
        public long Max { get; }
    }

    public class Streaming
    {
        public Streaming(bool renderer, bool proxy) => (Renderer, Proxy) = (renderer, proxy);

        [JsonProperty("renderer")]
        public bool Renderer { get; }

        [JsonProperty("proxy")]
        public bool Proxy { get; }
    }

    public class Config
    {
        public Config(string archetype, string function, string direction) =>
            (Archetype, Function, Direction) = (archetype, function, direction);

        [JsonProperty("archetype")]
        public string Archetype { get; }

        [JsonProperty("function")]
        public string Function { get; }

        [JsonProperty("direction")]
        public string Direction { get; }
    }

    public class State
    {
        public State(bool on, long bri, long hue, long sat, string effect, IEnumerable<double> xy, long ct, string alert, string colormode, string mode, bool reachable) =>
            (On, Brightness, Hue, Saturation, Effect, CIEColorCordinates, ColorTemperature, Alert, ColorMode, Mode, IsReachable) = (on, bri, hue, sat, effect, xy.ToList(), ct, alert, colormode, mode, reachable);


        [JsonProperty("on")]
        public bool On { get; }

        [JsonProperty("bri")]
        public long Brightness { get; }

        [JsonProperty("hue")]
        public long Hue { get; }

        [JsonProperty("sat")]
        public long Saturation { get; }

        [JsonProperty("effect")]
        public string Effect { get; }

        [JsonProperty("xy")]
        public List<double> CIEColorCordinates { get; }

        [JsonProperty("ct")]
        public long ColorTemperature { get; }

        [JsonProperty("alert")]
        public string Alert { get; } = "none";

        [JsonProperty("colormode")]
        public string ColorMode { get; }

        [JsonProperty("mode")]
        public string Mode { get; }

        [JsonProperty("reachable")]
        public bool IsReachable { get; }
    }

    public class LightModel
    {
        public LightModel(bool on, long bri, long hue, long sat) =>
            (On, Brightness, Hue, Saturation) = (on, bri, hue, sat);

        [JsonProperty("on")]
        public bool On { get; }

        [JsonProperty("bri")]
        public long Brightness { get; }

        [JsonProperty("hue")]
        public long Hue { get; }

        [JsonProperty("sat")]
        public long Saturation { get; }
    }

    public class Swupdate
    {
        public Swupdate(string state, DateTimeOffset lastinstall) =>
            (State, Lastinstall) = (state, lastinstall);

        [JsonProperty("state")]
        public string State { get; }

        [JsonProperty("lastinstall")]
        public DateTimeOffset Lastinstall { get; }
    }
}
