using System.Globalization;
using System.Windows.Data;

namespace Rayer.SearchEngine.Converters;

public class PlaylistCreatorConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        return value is string val
            ? $"by {val}"
            : value;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}