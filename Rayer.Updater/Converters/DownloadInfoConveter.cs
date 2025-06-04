using Rayer.Updater.Models;
using System.Globalization;
using System.Windows.Data;

namespace Rayer.Updater.Converters;

internal sealed class DownloadInfoConveter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is DownloadInfo { ReceiveBytes: > 0 } info)
        {
            return $"{info.ReceiveBytes}/{info.TotalBytes}";
        }

        return "加载数据中...";
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}