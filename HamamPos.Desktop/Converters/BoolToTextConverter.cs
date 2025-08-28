using System;
using System.Globalization;
using System.Windows.Data;

namespace HamamPos.Desktop.Converters
{
    // true/false değerini iki metinden birine çevirir.
    // ConverterParameter: "TrueText|FalseText" (örn: "Açık|Kapalı")
    public class BoolToTextConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var parts = (parameter as string)?.Split('|') ?? Array.Empty<string>();
            var trueText = parts.Length > 0 ? parts[0] : "True";
            var falseText = parts.Length > 1 ? parts[1] : "False";

            var b = value is bool v && v;
            return b ? trueText : falseText;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
            => throw new NotSupportedException();
    }
}
