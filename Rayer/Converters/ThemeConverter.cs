using System.Globalization;
using System.Windows.Data;
using Wpf.Ui.Appearance;

namespace Rayer.Converters;

internal sealed class ThemeConverter : IValueConverter
{
    public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        return value is ApplicationTheme.Dark
            ? 1
            : 0;
    }

    public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        return value is 1
            ? ApplicationTheme.Dark
            : ApplicationTheme.Light;
    }
}