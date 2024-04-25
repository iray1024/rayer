using System.IO;
using System.Net.Http;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace Rayer.Core.Utils;

public static class ImageSourceFactory
{
    private static readonly HttpClient _httpClient = new();

    public static ImageSource Create(string path)
    {
        var image = new BitmapImage();

        image.BeginInit();

        image.CacheOption = BitmapCacheOption.OnLoad;

        image.UriSource = new Uri(path);
        image.DecodePixelWidth = 256;

        image.EndInit();

        return image;
    }

    public static ImageSource CreateWebSource(string path)
    {
        var bytes = _httpClient.GetByteArrayAsync(path).Result;

        using var buffer = new MemoryStream(bytes);

        var image = new BitmapImage();

        image.BeginInit();

        image.CacheOption = BitmapCacheOption.OnLoad;

        image.StreamSource = buffer;
        image.DecodePixelWidth = 256;

        image.EndInit();

        return image;
    }

    public static async Task<ImageSource> CreateWebSourceAsync(Uri uri)
    {
        var bytes = await _httpClient.GetByteArrayAsync(uri);

        using var buffer = new MemoryStream(bytes);

        var image = new BitmapImage();

        image.BeginInit();

        image.CacheOption = BitmapCacheOption.OnLoad;

        image.StreamSource = buffer;
        image.DecodePixelWidth = 256;

        image.EndInit();

        return image;
    }
}