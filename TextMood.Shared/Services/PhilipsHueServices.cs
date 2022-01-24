using System;

namespace TextMood.Shared
{
    public static class PhilipsHueServices
    {
        public static int ConvertToHue(double red, double green, double blue)
        {
            double hue;
            var min = Math.Min(Math.Min(red, green), blue);
            var max = Math.Max(Math.Max(red, green), blue);

            switch (max)
            {
                case double number when areEqual(number, min):
                    return -1;

                case double number when areEqual(number, red):
                    hue = (green - blue) / (max - min);
                    break;

                case double _ when areEqual(max, green):
                    hue = 2f + (blue - red) / (max - min);
                    break;

                case double _ when areEqual(max, blue):
                    hue = 4f + (red - green) / (max - min);
                    break;

                default:
                    throw new NotSupportedException("Maximum Number Does Not Equal RGB Values");
            }

            hue *= 60;

            if (hue < 0)
                hue += 360;

            hue = (int)Math.Round(hue, 0);

            var hueForPhilips = (int)Math.Round(hue / 360 * 65535);
            return hueForPhilips;

            static bool areEqual(double num1, double num2) => Math.Abs(num1 - num2) < double.Epsilon;
        }
    }
}
