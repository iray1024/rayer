using System.Globalization;
using System.Windows.Data;

namespace Rayer.SearchEngine.Converters;

internal sealed class WebDurationConverter : IValueConverter
{
    public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is long duration)
        {
            var timeSpan = TimeSpan.FromMilliseconds(duration);

            return timeSpan.Hours == 0
                ? timeSpan.ToString(@"mm\:ss")
                : timeSpan.ToString(@"hh\:mm\:ss");
        }
        else if (value is TimeSpan timespan)
        {
            return timespan.Hours == 0
                ? timespan.ToString(@"mm\:ss")
                : timespan.ToString(@"hh\:mm\:ss");
        }

        return "00:00";
    }

    public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}