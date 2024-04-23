using Rayer.Core.Common;
using System.Globalization;
using System.Windows.Data;
using Wpf.Ui.Controls;

namespace Rayer.Converters;

internal class InfoBadgeConverter : IValueConverter
{
    private static readonly object _successEnumValueBox = InfoBadgeSeverity.Success;
    private static readonly object _informationalEnumValueBox = InfoBadgeSeverity.Informational;

    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        return value is null
            ? _informationalEnumValueBox
            : value?.Equals(BooleanBoxes.TrueBox) == true
                ? _successEnumValueBox
                : _informationalEnumValueBox;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}