﻿using System;
using System.Globalization;
using System.Windows.Data;

namespace HamamPos.Desktop.Converters
{
    // bool değeri tersine çevirir (true -> false).
    public class InverseBoolConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
            => value is bool b ? !b : value;

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
            => value is bool b ? !b : value;
    }
}
