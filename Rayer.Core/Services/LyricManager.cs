using Rayer.Core.Abstractions;
using Rayer.Core.Framework.Injection;
using Rayer.Core.Models;
using System.Collections.Concurrent;
using System.IO;
using System.Security.Cryptography;

namespace Rayer.Core.Services;
[Inject<ILyricManager>]
internal sealed class LyricManager : ILyricManager
{
    private readonly ConcurrentDictionary<string, int> _offsetMap = [];

    public LyricManager()
    {
        if (!Directory.Exists(Constants.Paths.LyricPath))
        {
            Directory.CreateDirectory(Constants.Paths.LyricPath);
        }
        else
        {
            _offsetMap = new ConcurrentDictionary<string, int>(Directory.EnumerateFiles(Constants.Paths.LyricPath, "*")
                .Select(x => x.Split('_'))
                .ToDictionary(k => Path.GetFileNameWithoutExtension(k[0]), v => int.Parse(v[1])));
        }
    }

    public void Store(Audio audio, int offset)
    {
        var md5 = GetMD5(audio);

        int finalOffset = offset;
        var files = Directory.GetFiles(Constants.Paths.LyricPath, $"{md5}*", SearchOption.TopDirectoryOnly);
        if (files.Length == 0)
        {
            using var newFile = new FileStream($"{Constants.Paths.LyricPath}/{md5}_{finalOffset}", FileMode.CreateNew, FileAccess.Write, FileShare.None);
            newFile.Close();
        }
        else
        {
            var fileInfo = new FileInfo(files[0]);
            var originalOffset = int.Parse(fileInfo.Name.Split('_')[1]);
            finalOffset = originalOffset + offset;
            fileInfo.MoveTo($"{Constants.Paths.LyricPath}/{md5}_{finalOffset}");
        }

        _offsetMap.AddOrUpdate(md5, finalOffset, (_, _) => finalOffset);
    }

    public int LoadOffset(Audio audio)
    {
        var md5 = GetMD5(audio);

        if (_offsetMap.TryGetValue(md5, out var offset))
        {
            return offset;
        }

        return 0;
    }

    public static string GetMD5(Audio audio)
    {
        var data = Encoding.UTF8.GetBytes($"{audio.Title}{audio.Album}{audio.Path}");
        var hash = CryptographicOperations.HashData(HashAlgorithmName.MD5, data);
        var md5String = Convert.ToHexString(hash);

        return md5String.ToLower();
    }
}