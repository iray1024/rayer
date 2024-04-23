using Rayer.Core.Common;
using System.Globalization;
using System.Windows.Data;
using Wpf.Ui.Appearance;

namespace Rayer.Converters;

internal sealed class ThemeConverter : IValueConverter
{
    private static readonly object _lightEnumValueBox = ApplicationTheme.Light;
    private static readonly object _darkEnumValueBox = ApplicationTheme.Dark;

    public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        return value?.Equals(_darkEnumValueBox) == true
            ? Int32Boxes.OneValueBox
            : Int32Boxes.ZeroValueBox;
    }

    public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        return value?.Equals(Int32Boxes.OneValueBox) == true
            ? _darkEnumValueBox
            : _lightEnumValueBox;
    }
}