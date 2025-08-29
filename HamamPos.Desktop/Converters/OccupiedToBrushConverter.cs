using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Windows.Data;
using System.Windows.Media;
using HamamPos.Shared.Dtos;
using HamamPos.Shared.Models;

namespace HamamPos.Desktop.Converters
{
    /// <summary>
    /// Unit (ServiceUnit) + OccupiedUnitIds bilgisinden buton fırçası üretir.
    /// MultiBinding ile: ConverterParameter kullanılmıyor.
    /// </summary>
    public class OccupiedToBrushConverter : IMultiValueConverter
    {
        private static readonly Brush OccupiedBrush = new SolidColorBrush(Color.FromRgb(245, 91, 91)); // kırmızı
        private static readonly Brush FreeBrush = new SolidColorBrush(Color.FromRgb(240, 240, 240)); // açık gri
        private static readonly Brush BorderBrush = new SolidColorBrush(Color.FromRgb(60, 60, 60));

        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            // values[0]: ServiceUnit
            // values[1]: HashSet<int> veya List<int> (Occupied Unit Ids)
            if (values.Length < 2 || values[0] is not ServiceUnit unit) return FreeBrush;

            var ids = values[1] as IEnumerable<int>;
            var isOcc = ids != null && ids.Contains(unit.Id);

            // Arka plan için
            if (targetType == typeof(Brush))
                return isOcc ? OccupiedBrush : FreeBrush;

            // Kenarlık vb. farklı hedef olursa basitçe Free döndür
            return FreeBrush;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
            => throw new NotSupportedException();
    }
}
