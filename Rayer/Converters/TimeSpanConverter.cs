using System.Globalization;
using System.Windows.Data;

namespace Rayer.Converters;

internal sealed class TimeSpanConverter : IValueConverter
{
    public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        return value is TimeSpan time
            ? time.Hours == 0
                ? time.ToString(@"mm\:ss")
                : (object)time.ToString(@"hh\:mm\:ss")
            : "Unknown";
    }

    public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}