using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;

namespace Rayer.Converters;

internal class BrushToColorConverter : IValueConverter
{
    public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        return value is SolidColorBrush brush
            ? brush.Color
            : value is Color
            ? value
            : new Color
            {
                A = 255,
                R = 255,
                G = 0,
                B = 0
            };
    }

    public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}