using Rayer.Core.Common;
using System.Globalization;
using System.Windows.Data;

namespace Rayer.Converters;

internal sealed class ImmersiveModeConverter : IValueConverter
{
    private static readonly object _audioVisualizerEnumValueBox = ImmersiveMode.AudioVisualizer;
    private static readonly object _vinylEnumValueBox = ImmersiveMode.Vinyl;

    public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        return value?.Equals(_vinylEnumValueBox) == true
            ? Int32Boxes.ZeroValueBox
            : Int32Boxes.OneValueBox;
    }

    public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        return value?.Equals(Int32Boxes.OneValueBox) == true
            ? _audioVisualizerEnumValueBox
            : _vinylEnumValueBox;
    }
}