using System.Globalization;

namespace Tetris.Converters
{
    public class ColorNameToColorConverter : IValueConverter
    {
        public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            if (value is string name)
            {
                return name switch
                {
                    "Red" => Colors.Red,
                    "Orange" => Colors.Orange,
                    "Yellow" => Colors.Yellow,
                    "Green" => Colors.Green,
                    "Blue" => Colors.Blue,
                    "Indigo" => Colors.Indigo,
                    "Violet" => Colors.Violet,
                    "Light Blue" => Colors.LightBlue,
                    _ => Colors.Black
                };
            }
            return Colors.Black;
        }

        public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
