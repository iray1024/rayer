using Rayer.Core.Common;
using System.Globalization;
using System.Windows.Data;

namespace Rayer.SearchEngine.Converters;

public class AdjustPanelRowConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        return value is int count && parameter is string colCount
            ? Math.Ceiling(1.0 * count / int.Parse(colCount) * 1.0)
            : Int32Boxes.OneValueBox;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}