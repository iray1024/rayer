using Rayer.Core.Common;
using System.Globalization;
using System.Windows.Data;

namespace Rayer.SearchEngine.Converters;

internal class SearcherTypeConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        return value is SearcherType searcherType
            ? searcherType switch
            {
                SearcherType.Netease => 0,
                SearcherType.Bilibili => 1,
                _ => 0,
            }
            : 0;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        return value is int val
            ? val switch
            {
                0 => SearcherType.Netease,
                1 => SearcherType.Bilibili,
                _ => SearcherType.Netease
            }
            : SearcherType.Netease;
    }
}