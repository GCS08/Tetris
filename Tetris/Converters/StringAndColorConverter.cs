using System.Globalization;
using Microsoft.Maui.Graphics;
using Tetris.Models;

namespace Tetris.Converters
{
    /// <summary>
    /// Converter that translates between a string color key (stored in the model)
    /// and a MAUI Color object (used by the UI).
    /// Enables binding between UI elements and string-based color values.
    /// </summary>
    public class StringAndColorConverter : IValueConverter
    {
        /// <summary>
        /// Converts a color key string into its corresponding MAUI Color object.
        /// Used when the UI needs to display a color that is stored as a string key.
        /// </summary>
        /// <param name="input">The color key string.</param>
        /// <returns>The matching MAUI Color. Returns Transparent if the key is unknown.</returns>
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
                Keys.VioletKey => Colors.Violet,
                Keys.LightBlueKey => Colors.LightBlue,
                _ => Colors.Transparent
            };
            return color;
        }

        /// <summary>
        /// Converts a MAUI Color object into its corresponding string key.
        /// Used when saving or sending color data that is represented as a string.
        /// </summary>
        /// <param name="color">The MAUI Color to convert.</param>
        /// <returns>The string key representing the color.</returns>
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

        /// <summary>
        /// Converts a string color key from the binding source into a MAUI Color
        /// for use in the UI.
        /// </summary>
        /// <param name="value">The color key string.</param>
        /// <param name="targetType">The type expected by the binding target.</param>
        /// <param name="parameter">Optional binding parameter.</param>
        /// <param name="culture">Culture information.</param>
        /// <returns>The corresponding MAUI Color.</returns>
        public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            return ColorNameToColor((string)value!);
        }

        /// <summary>
        /// Converts a MAUI Color from the UI back into its string key representation.
        /// Used when updating the underlying data source.
        /// </summary>
        /// <param name="value">The MAUI Color value.</param>
        /// <param name="targetType">The type expected by the binding source.</param>
        /// <param name="parameter">Optional binding parameter.</param>
        /// <param name="culture">Culture information.</param>
        /// <returns>The string key representing the color.</returns>
        public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            return ColorToColorName((Color)value!);
        }
    }
}