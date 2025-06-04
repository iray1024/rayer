using System.Globalization;
using System.Windows.Data;

namespace Rayer.Updater.Converters;

internal sealed class DownloadSpeedConveter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is double speed)
        {
            return $"{Math.Round(speed / 1000000, 2)} MB/s";
        }

        return string.Empty;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}