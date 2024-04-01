using Rayer.Core.Models;
using System.IO;
using System.Windows.Media.Imaging;

namespace Rayer.Core.Utils;

public static class MediaRecognizer
{
    public static Audio Recognize(string path)
    {
        using var tfile = TagLib.File.Create(path);

        var fileName = Path.GetFileNameWithoutExtension(path);

        var tag = tfile.Tag;

        var model = new Audio()
        {
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
}