using Rayer.Core.Common;
using System.Globalization;
using System.Windows.Data;

namespace Rayer.Converters;

internal sealed class PlaySingleAudioStrategyConverter : IValueConverter
{
    public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        return value is PlaySingleAudioStrategy.AddToQueue
            ? 0
            : 1;
    }

    public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        return value is 1
            ? PlaySingleAudioStrategy.ReplacePlayQueue
            : PlaySingleAudioStrategy.AddToQueue;
    }
}