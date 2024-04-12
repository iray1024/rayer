using System.Globalization;
using System.Windows.Data;

namespace Rayer.SearchEngine.Internal.Converters;

public class DynamicIslandHeightConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        return value is double val ? val + 26 : value;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}