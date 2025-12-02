using System.Globalization;
using Microsoft.Maui.Graphics;
using Tetris.Models;

namespace Tetris.Converters
{
    public class StringAndColorConverter
    {
        public static Color ColorNameToColor(string input)
        {
            if (input == Keys.RedKey) return Colors.Red;
            if (input == Keys.OrangeKey) return Colors.Orange;
            if (input == Keys.YellowKey) return Colors.Yellow;
            if (input == Keys.GreenKey) return Colors.Green;
            if (input == Keys.BlueKey) return Colors.Blue;
            if (input == Keys.IndigoKey) return Colors.Indigo;
            if (input == Keys.VioletKey) return Colors.Violet;
            if (input == Keys.LightBlueKey) return Colors.LightBlue;
            return Colors.Transparent;
        }

        public static string ColorToColorName(Color color)
        {
            if (color == Colors.Red) return Keys.RedKey;
            if (color == Colors.Orange) return Keys.OrangeKey;
            if (color == Colors.Yellow) return Keys.YellowKey;
            if (color == Colors.Green) return Keys.GreenKey;
            if (color == Colors.Blue) return Keys.BlueKey;
            if (color == Colors.Indigo) return Keys.IndigoKey;
            if (color == Colors.Violet) return Keys.VioletKey;
            if (color == Colors.LightBlue) return Keys.LightBlueKey;
            return Keys.TransparentKey;
        }
    }
}
