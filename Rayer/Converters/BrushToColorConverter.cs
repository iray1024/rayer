using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;

namespace Rayer.Converters;

internal class BrushToColorConverter : IValueConverter
{
    public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is SolidColorBrush brush)
        {
            return brush.Color;
        }

        return value is Color
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