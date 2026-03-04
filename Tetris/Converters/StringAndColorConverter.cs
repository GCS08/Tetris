using System.Globalization;
using Microsoft.Maui.Graphics;
using Tetris.Models;

namespace Tetris.Converters
{
    public class StringAndColorConverter
    {
        public static Color ColorNameToColor(string input)
        {
            Color color = input switch
            {
                Keys.RedKey => Colors.Red,
                Keys.OrangeKey => Colors.Orange,
                Keys.YellowKey => Colors.Yellow,
                Keys.GreenKey => Colors.Green,
                Keys.BlueKey => Colors.Blue,
                Keys.IndigoKey => Colors.Indigo,
                Keys.VioletKey=> Colors.Violet,
                Keys.LightBlueKey => Colors.LightBlue,
                _ => Colors.Transparent
            };
            return color;
        }

        public static string ColorToColorName(Color color)
        {
            string colorFinal = Keys.TransparentKey;
            if (color == Colors.Red) colorFinal = Keys.RedKey;
            else if (color == Colors.Orange) colorFinal = Keys.OrangeKey;
            else if (color == Colors.Yellow) colorFinal = Keys.YellowKey;
            else if (color == Colors.Green) colorFinal = Keys.GreenKey;
            else if (color == Colors.Blue) colorFinal = Keys.BlueKey;
            else if (color == Colors.Indigo) colorFinal = Keys.IndigoKey;
            else if (color == Colors.Violet) colorFinal = Keys.VioletKey;
            else if (color == Colors.LightBlue) colorFinal = Keys.LightBlueKey;
            return colorFinal;
        }
    }
}
