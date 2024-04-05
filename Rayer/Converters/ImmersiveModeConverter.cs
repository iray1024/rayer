using Rayer.Core.Common;
using System.Globalization;
using System.Windows.Data;

namespace Rayer.Converters;

internal sealed class ImmersiveModeConverter : IValueConverter
{
    public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        return value is ImmersiveMode.Vinyl
            ? 0
            : 1;
    }

    public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        return value is 1
            ? ImmersiveMode.AudioVisualizer
            : ImmersiveMode.Vinyl;
    }
}