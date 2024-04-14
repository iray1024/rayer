using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace Rayer.Core.Utils;

public static class ImageSourceUtils
{
    public static ImageSource Create(string path)
    {
        var bmp = new BitmapImage();

        bmp.BeginInit();

        bmp.CacheOption = BitmapCacheOption.OnLoad;

        bmp.UriSource = new Uri(path);

        bmp.EndInit();

        return bmp;
    }
}