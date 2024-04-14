using System.Globalization;
using System.Windows.Data;

namespace Rayer.SearchEngine.Converters;

public class DynamicIslandWidthConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        return value is double val ? val + 20 : value;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}