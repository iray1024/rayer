using Rayer.Core.Abstractions;
using Rayer.Core.Common;
using Rayer.Core.Framework.Injection;
using Rayer.Core.Framework.Settings.Abstractions;
using Rayer.Core.Models;
using System.Collections.Concurrent;
using System.IO;
using System.Security.Cryptography;

namespace Rayer.Core.Services;
[Inject<ILyricManager>]
internal sealed class LyricManager : ILyricManager
{
    private readonly ISettingsService _settingsService;
    private readonly ConcurrentDictionary<LyricKey, int> _offsetMap = [];

    public LyricManager(ISettingsService settingsService)
    {
        _settingsService = settingsService;

        if (!Directory.Exists(Constants.Paths.LyricPath))
        {
            Directory.CreateDirectory(Constants.Paths.LyricPath);
        }
        else
        {
            _offsetMap = new ConcurrentDictionary<LyricKey, int>(Directory.EnumerateFiles(Constants.Paths.LyricPath, "*")
                .Select(x => x.Split('_'))
                .ToDictionary(k => new LyricKey(Path.GetFileNameWithoutExtension(k[0]), (LyricSearcher)int.Parse(k[1])), v => int.Parse(v[2])));
        }
    }

    public void Store(Audio audio, int offset)
    {
        var md5 = GetMD5(audio);
        var filenamePrefix = $"{md5}_{(int)_settingsService.Settings.LyricSearcher}";

        int finalOffset = offset;
        var files = Directory.GetFiles(Constants.Paths.LyricPath, $"{filenamePrefix}*", SearchOption.TopDirectoryOnly);
        if (files.Length == 0)
        {
            using var newFile = new FileStream($"{Constants.Paths.LyricPath}/{md5}_{(int)_settingsService.Settings.LyricSearcher}_{finalOffset}", FileMode.CreateNew, FileAccess.Write, FileShare.None);
            newFile.Close();
        }
        else
        {
            var fileInfo = new FileInfo(files[0]);
            var originalOffset = int.Parse(fileInfo.Name.Split('_')[2]);
            finalOffset = originalOffset + offset;
            fileInfo.MoveTo($"{Constants.Paths.LyricPath}/{md5}_{(int)_settingsService.Settings.LyricSearcher}_{finalOffset}");
        }

        _offsetMap.AddOrUpdate(new LyricKey(md5, _settingsService.Settings.LyricSearcher), finalOffset, (_, _) => finalOffset);
    }

    public int LoadOffset(Audio audio)
    {
        var md5 = GetMD5(audio);

        if (_offsetMap.TryGetValue(new LyricKey(md5, _settingsService.Settings.LyricSearcher), out var offset))
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

    private record LyricKey(string MD5, LyricSearcher LyricSearcher);
}