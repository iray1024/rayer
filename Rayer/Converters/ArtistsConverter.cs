using System.Globalization;
using System.Windows.Data;

namespace Rayer.Converters;

internal sealed class ArtistsConverter : IValueConverter
{
    public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        return value is string[] artists
            ? string.Join(" / ", artists)
            : "Unknown";
    }

    public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}