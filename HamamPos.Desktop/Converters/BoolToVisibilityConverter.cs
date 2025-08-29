using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace HamamPos.Desktop.Converters
{
    /// <summary>
    /// bool -> Visibility (true=>Visible, false=>Collapsed)
    /// Parametre "Invert" verilirse tersler (true=>Collapsed).
    /// </summary>
    public class BoolToVisibilityConverter : IValueConverter
    {
        public bool Invert { get; set; } = false;

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var b = value is bool v && v;
            if (Invert) b = !b;
            return b ? Visibility.Visible : Visibility.Collapsed;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
            => throw new NotSupportedException();
    }
}
