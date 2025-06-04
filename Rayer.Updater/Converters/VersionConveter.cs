using System.Globalization;
using System.Windows.Data;

namespace Rayer.Updater.Converters;

internal sealed class VersionConveter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is string { Length: > 0 })
        {
            if (parameter.Equals("0"))
            {
                return $"当前版本: {value}";
            }
            else
            {
                return $"最新版本: {value}";
            }
        }

        return parameter.Equals("0") ? "版本检测中..." : string.Empty;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}