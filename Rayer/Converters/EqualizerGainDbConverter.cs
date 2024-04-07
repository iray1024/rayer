using System.Globalization;
using System.Windows.Data;

namespace Rayer.Converters;

internal sealed class EqualizerGainDbConverter : IValueConverter
{
    public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        return value is float db
            ? $"{db}db"
            : string.Empty;
    }

    public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}