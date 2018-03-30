using System.Net.Http;
using System.Threading.Tasks;

using Plugin.Settings;

namespace TextMood
{
	abstract class PhillipsHueBridgeServices : BaseHttpClientService
	{
		public static string PhillipsHueBridgeIPAddress
		{
			get => CrossSettings.Current.GetValueOrDefault(nameof(PhillipsHueBridgeIPAddress), string.Empty);
			set => CrossSettings.Current.AddOrUpdateValue(nameof(PhillipsHueBridgeIPAddress), value);
		}

		public static Task<HttpResponseMessage> UpdateLightBulbColor(int hue)
		{
			var hueRequest = new PhillipsHueRequestModel
			{
				On = true,
				Hue = hue,
				Saturation = hue >= 0 ? 255 : 0,
				Brightness = 255
			};

			return PutObjectToAPI($"http://{PhillipsHueBridgeIPAddress}/api/5pE71iepzEeuKQY1SSfwuiATTfih3dy0YZDAwhCh/lights/1/state", hueRequest);
		}
	}
}
