using Rayer.Core.Common;
using System.Globalization;
using System.Windows.Data;

namespace Rayer.Converters;

internal sealed class PitchProviderConverter : IValueConverter
{
    public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        return value is PitchProvider.NAudio
            ? 0
            : 1;
    }

    public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        return value is 1
            ? PitchProvider.SoundTouch
            : PitchProvider.NAudio;
    }
}