using Rayer.Core.Common;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace Rayer.Core.Converters;

internal class BoolToVisibilityConverter : IValueConverter
{
    public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        return value?.Equals(BooleanBoxes.TrueBox) == true ? Visibility.Visible : Visibility.Collapsed;
    }

    public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}