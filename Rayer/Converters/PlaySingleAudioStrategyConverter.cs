using Rayer.Core.Common;
using System.Globalization;
using System.Windows.Data;

namespace Rayer.Converters;

internal sealed class PlaySingleAudioStrategyConverter : IValueConverter
{
    private static readonly object _addToQueueEnumValueBox = PlaySingleAudioStrategy.AddToQueue;
    private static readonly object _replaceEnumValueBox = PlaySingleAudioStrategy.ReplacePlayQueue;

    public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        return value?.Equals(_addToQueueEnumValueBox) == true
            ? Int32Boxes.ZeroValueBox
            : Int32Boxes.OneValueBox;
    }

    public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        return value?.Equals(Int32Boxes.OneValueBox) == true
            ? _replaceEnumValueBox
            : _addToQueueEnumValueBox;
    }
}