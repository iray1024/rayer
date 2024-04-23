using Rayer.Core.Common;
using System.Globalization;
using System.Windows.Data;

namespace Rayer.Converters;

internal sealed class LyricSearcherConverter : IValueConverter
{
    private static readonly object _neteaseEnumValueBox = LyricSearcher.Netease;
    private static readonly object _qqEnumValueBox = LyricSearcher.QQMusic;
    private static readonly object _kugouEnumValueBox = LyricSearcher.Kugou;

    public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        return value?.Equals(_neteaseEnumValueBox) == true
            ? Int32Boxes.ZeroValueBox
            : value?.Equals(_qqEnumValueBox) == true
                ? Int32Boxes.OneValueBox
                : Int32Boxes.TwoValueBox;
    }

    public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        return value?.Equals(Int32Boxes.ZeroValueBox) == true
            ? _neteaseEnumValueBox
            : value?.Equals(Int32Boxes.OneValueBox) == true
                ? _qqEnumValueBox
                : _kugouEnumValueBox;
    }
}