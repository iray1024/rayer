using Rayer.Core.Common;
using System.Globalization;
using System.Windows.Data;

namespace Rayer.SearchEngine.Converters;

internal class SearcherTypeConverter : IValueConverter
{
    private static readonly object _neteaseEnumValueBox = SearcherType.Netease;
    private static readonly object _bilibiliEnumValueBox = SearcherType.Bilibili;

    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        return value is SearcherType searcherType
            ? searcherType switch
            {
                SearcherType.Netease => Int32Boxes.ZeroValueBox,
                SearcherType.Bilibili => Int32Boxes.OneValueBox,
                _ => Int32Boxes.ZeroValueBox
            }
            : Int32Boxes.ZeroValueBox;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        return value is int val
            ? val switch
            {
                0 => _neteaseEnumValueBox,
                1 => _bilibiliEnumValueBox,
                _ => _neteaseEnumValueBox
            }
            : _neteaseEnumValueBox;
    }
}