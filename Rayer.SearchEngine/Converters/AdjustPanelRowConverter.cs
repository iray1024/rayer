using System.Globalization;
using System.Windows.Data;

namespace Rayer.SearchEngine.Converters;

public class AdjustPanelRowConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        return value is int count && parameter is string colCount
            ? Math.Ceiling(count / int.Parse(colCount) * 1.0)
            : 1;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}