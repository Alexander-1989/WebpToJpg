using System;
using System.Text;
using System.Drawing;

namespace WebpToJpg.ColorServise
{
    public static class ColorConvert
    {
        public static string ColorToHex(Color color)
        {
            StringBuilder result = new StringBuilder("#", 7);
            if (color.R < 10) result.Append($"0{color.R}"); else result.Append($"{color.R:X}");
            if (color.G < 10) result.Append($"0{color.G}"); else result.Append($"{color.G:X}");
            if (color.B < 10) result.Append($"0{color.B}"); else result.Append($"{color.B:X}");
            return result.ToString();
        }

        public static Color ColorFromHex(string str)
        {
            try
            {
                int index = str.LastIndexOf('#');
                if (index > -1)
                {
                    byte R = Convert.ToByte(str.Substring(index + 1, 2), 16);
                    byte G = Convert.ToByte(str.Substring(index + 3, 2), 16);
                    byte B = Convert.ToByte(str.Substring(index + 5, 2), 16);
                    return Color.FromArgb(R, G, B);
                }
            }
            catch (Exception) { }
            return Color.Empty;
        }
    }
}