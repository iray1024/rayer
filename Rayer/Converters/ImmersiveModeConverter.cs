using Rayer.Core.Common;
using System.Globalization;
using System.Windows.Data;

namespace Rayer.Converters;

internal sealed class ImmersiveModeConverter : IValueConverter
{
    private static readonly object _audioVisualizerEnumValueBox = ImmersiveMode.AudioVisualizer;
    private static readonly object _vinylEnumValueBox = ImmersiveMode.Vinyl;
    private static readonly object _albumEnumValueBox = ImmersiveMode.Album;

    public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        return value?.Equals(_vinylEnumValueBox) == true
            ? Int32Boxes.ZeroValueBox
            : value?.Equals(_audioVisualizerEnumValueBox) == true
                ? Int32Boxes.OneValueBox
                : Int32Boxes.TwoValueBox;
    }

    public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        return value?.Equals(Int32Boxes.ZeroValueBox) == true
            ? _vinylEnumValueBox
            : value?.Equals(Int32Boxes.OneValueBox) == true
                ? _audioVisualizerEnumValueBox
                : _albumEnumValueBox;
    }
}