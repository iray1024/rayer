using System.Globalization;
using System.Windows.Data;
using Wpf.Ui.Controls;

namespace Rayer.Converters;

internal class InfoBadgeConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        return value is bool val
            ? val
                ? InfoBadgeSeverity.Success
                : InfoBadgeSeverity.Informational
            : InfoBadgeSeverity.Informational;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}