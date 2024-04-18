using System.Globalization;
using System.Windows.Data;

namespace Rayer.SearchEngine.Converters;

internal sealed class PainedLyricConverter : IValueConverter
{
    public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        return value is string[] { Length: 4 } lyrics && parameter is string index
            ? lyrics[int.Parse(index)]
            : string.Empty;
    }

    public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}