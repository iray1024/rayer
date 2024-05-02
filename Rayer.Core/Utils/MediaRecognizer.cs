using Rayer.Core.Framework;
using Rayer.Core.Models;
using System.IO;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace Rayer.Core.Utils;

public static class MediaRecognizer
{
    private static IXXH64 _xxh = null!;

    public static Audio Recognize(string path)
    {
        using var tfile = TagLib.File.Create(path);

        var fileName = Path.GetFileNameWithoutExtension(path);

        var tag = tfile.Tag;

        _xxh.Update(new Memory<byte>(Encoding.UTF8.GetBytes(fileName)).ToArray());
        var hash = Convert.ToBase64String(_xxh.DigestBytes());
        _xxh.Reset();

        var model = new Audio()
        {
            Id = hash,
            Artists = tag.Performers,
            Album = tag.Album,
            Path = path,
            Title = tag.Title,
            Duration = tfile.Properties.Duration
        };

        if (model.Artists.Length == 0)
        {
            model.Artists = ["未知歌手"];
        }

        if (string.IsNullOrEmpty(model.Title))
        {
            model.Title = fileName;
        }

        if (string.IsNullOrEmpty(model.Album))
        {
            model.Album = fileName;
        }

        if (tag.Pictures.Length > 0)
        {
            try
            {
                var cover = tag.Pictures[0];

                var bmp = new BitmapImage();

                bmp.BeginInit();

                bmp.CacheOption = BitmapCacheOption.OnLoad;
                using var ms = new MemoryStream(cover.Data.Data);
                bmp.StreamSource = ms;

                bmp.EndInit();
                bmp.Freeze();

                model.Cover = bmp;
            }
            catch
            {

            }
        }

        tfile.Dispose();

        return model;
    }

    public static ImageSource? ExtractCover(string path)
    {
        try
        {
            using var tfile = TagLib.File.Create(path);

            var fileName = Path.GetFileNameWithoutExtension(path);

            var tag = tfile.Tag;

            if (tag.Pictures.Length > 0)
            {
                try
                {
                    var cover = tag.Pictures[0];

                    var bmp = new BitmapImage();

                    bmp.BeginInit();

                    bmp.CacheOption = BitmapCacheOption.OnLoad;
                    using var ms = new MemoryStream(cover.Data.Data);
                    bmp.StreamSource = ms;

                    bmp.EndInit();
                    bmp.Freeze();

                    return bmp;
                }
                catch
                {
                    return null;
                }
            }
        }
        catch
        {
            return null;
        }

        return null;
    }

    public static void Initialize()
    {
        _xxh = AppCore.GetRequiredService<IXXH64>();
    }
}