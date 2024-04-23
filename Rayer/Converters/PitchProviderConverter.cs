using Rayer.Core.Common;
using System.Globalization;
using System.Windows.Data;

namespace Rayer.Converters;

internal sealed class PitchProviderConverter : IValueConverter
{
    private static readonly object _naudioEnumValueBox = PitchProvider.NAudio;
    private static readonly object _soundTouchEnumValueBox = PitchProvider.SoundTouch;

    public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        return value?.Equals(_naudioEnumValueBox) == true
            ? Int32Boxes.ZeroValueBox
            : Int32Boxes.OneValueBox;
    }

    public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        return value?.Equals(Int32Boxes.OneValueBox) == true
            ? _soundTouchEnumValueBox
            : _naudioEnumValueBox;
    }
}