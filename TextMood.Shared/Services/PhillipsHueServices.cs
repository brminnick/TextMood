using System;
namespace TextMood.Shared
{
	public static class PhillipsHueServices
	{
		public static int ConvertToHue(float red, float green, float blue)
		{
			float hue;
			var min = Math.Min(Math.Min(red, green), blue);
			var max = Math.Max(Math.Max(red, green), blue);

			switch (max)
			{
				case float number when (number == min):
					return -1;

				case float number when (number == red):
					hue = (green - blue) / (max - min);
					break;

				case float number when (max == green):
					hue = 2f + (blue - red) / (max - min);
					break;

				case float number when (max == blue):
					hue = 4f + (red - green) / (max - min);
					break;

				default:
					throw new NotSupportedException("Maximum Number Does Not Equal RGB Values");
			}

			hue = hue * 60;

			if (hue < 0)
				hue = hue + 360;

			hue = (int)Math.Round(hue, 0);

			var hueForPhillips = (int)Math.Round(hue / 360 * 65535);
			return hueForPhillips;
		}
	}
}
