using Rayer.Core.Utils;
using Rayer.SearchEngine.Core.Enums;
using System.Globalization;
using System.Windows.Data;

namespace Rayer.SearchEngine.Converters;

internal class SearchTypeConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        return parameter is string val ? EnumHelper.GetDescription<SearchType>(val) : value;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}