using System.Globalization;
using System.Windows.Data;

namespace Rayer.SearchEngine.Converters;

internal class CompanyConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        return value is string { Length: > 0 } company ? $"©{company}" : value;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}