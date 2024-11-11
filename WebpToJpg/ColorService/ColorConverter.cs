using System;
using System.Text;
using System.Drawing;

namespace WebpToJpg.ColorService
{
    public static class ColorConvert
    {
        public static string ColorToHex(Color color)
        {
            StringBuilder result = new StringBuilder("#", 7);
            result.Append(color.R < 10 ? $"0{color.R}" : $"{color.R:X}");
            result.Append(color.G < 10 ? $"0{color.G}" : $"{color.G:X}");
            result.Append(color.B < 10 ? $"0{color.B}" : $"{color.B:X}");
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