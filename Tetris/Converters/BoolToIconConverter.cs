using Tetris.Models;
using System.Globalization;

namespace Tetris.Converters
{
    /// <summary>
    /// Converts a boolean value to the corresponding visibility icon.
    /// Used for bindings where a password visibility state determines 
    /// which icon (visible / hidden) should be displayed in the UI.
    /// </summary>
    public class BoolToIconConverter : IValueConverter
    {
        #region Public Methods
        /// <summary>
        /// Converts a boolean value into the corresponding icon string.
        /// If the value is true, the "visibility off" icon is returned.
        /// If the value is false, the "visibility on" icon is returned.
        /// </summary>
        /// <param name="value">The boolean value representing the current visibility state.</param>
        /// <param name="targetType">The type expected by the binding target.</param>
        /// <param name="parameter">Optional parameter supplied by the binding.</param>
        /// <param name="culture">The culture information for the conversion.</param>
        /// <returns>A string representing the icon that should be displayed.</returns>
        public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            string icon = Icons.Visibility_off;
            if (value != null)
                icon = (bool)value ? Icons.Visibility_off : Icons.Visibility_on;
            return icon;
        }

        /// <summary>
        /// Converts the icon value back to the original boolean value.
        /// This converter does not support reverse conversion and always returns null.
        /// </summary>
        /// <param name="value">The value from the binding target.</param>
        /// <param name="targetType">The type expected by the binding source.</param>
        /// <param name="parameter">Optional parameter supplied by the binding.</param>
        /// <param name="culture">The culture information for the conversion.</param>
        /// <returns>Always returns null because reverse conversion is not implemented.</returns>
        public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            return null;
        }
        #endregion
    }
}