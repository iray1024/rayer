using Rayer.Core.Common;
using System.Globalization;
using System.Windows.Data;

namespace Rayer.Converters;

internal sealed class LyricSearcherConverter : IValueConverter
{
    public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        return value is LyricSearcher.Netease
            ? 0
            : value is LyricSearcher.QQMusic
                ? 1
                : 2;
    }

    public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        return value is 0
            ? LyricSearcher.Netease
            : value is 1 ? LyricSearcher.QQMusic : LyricSearcher.Kugou;
    }
}