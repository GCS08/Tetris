using System.Globalization;
using Microsoft.Maui.Graphics;

namespace Tetris.Converters
{
    public class RGBAStringToColorConverter
    {
        public static Color RGBAStringToColor(string input)
        {
            input = input.Replace("[Color:", "")
                         .Replace("]", "")
                         .Trim();
            string[] parts = input.Split(',');

            float r = 0, g = 0, b = 0, a = 1;

            foreach (string part in parts)
            {
                string[] kv = part.Split('=');
                string key = kv[0].Trim();
                float value = float.Parse(kv[1], CultureInfo.InvariantCulture);

                switch (key)
                {
                    case "Red": r = value; break;
                    case "Green": g = value; break;
                    case "Blue": b = value; break;
                    case "Alpha": a = value; break;
                }
            }

            return Color.FromRgba(r, g, b, a);
        }
    }
}
