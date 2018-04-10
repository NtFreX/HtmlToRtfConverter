using System.Drawing;

namespace NtFreX.HtmlToRtfConverter
{
    public static class HtmlColorHelper
    {
        public static Color GetColor(string htmlColor)
        {
            if (htmlColor.StartsWith("#"))
            {
                var red = htmlColor.Substring(1, 2);
                var green = htmlColor.Substring(3, 2);
                var blue = htmlColor.Substring(5, 2);

                return Color.FromArgb(1,
                    int.Parse(red, System.Globalization.NumberStyles.HexNumber),
                    int.Parse(green, System.Globalization.NumberStyles.HexNumber),
                    int.Parse(blue, System.Globalization.NumberStyles.HexNumber));
            }

            return Color.FromName(htmlColor);
        }
    }
}