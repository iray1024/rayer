using Rayer.Core.Abstractions;
using Rayer.Core.Models;
using Rayer.FrameworkCore.Injection;
using System.IO;

namespace Rayer.Core.Services;

[Inject<ICoverManager>]
internal sealed class CoverManager : ICoverManager
{
    private static readonly string _coverPath = Path.Combine(Constants.Paths.AppDataDir, "cover.dat");
    private Dictionary<string, string> _meidaMap = [];

    public CoverManager()
    {
        if (File.Exists(_coverPath))
        {
            var coverMaps = File.ReadAllLines(_coverPath);
            _meidaMap = new Dictionary<string, string>(coverMaps.Select(x =>
            {
                var slice = x.Split(',');

                return KeyValuePair.Create(slice[0], slice[1]);
            }));
        }
    }

    public event EventHandler<Audio>? CoverChanged;

    public string? GetCover(Audio audio)
    {
        if (_meidaMap.TryGetValue(audio.Id, out var path))
        {
            return path;
        }

        return null;
    }

    public async Task SetCoverAsync(Audio audio, string mediaSource)
    {
        if (_meidaMap.TryGetValue(audio.Id, out _))
        {
            _meidaMap[audio.Id] = mediaSource;
        }
        else
        {
            _meidaMap.Add(audio.Id, mediaSource);
        }

        await UpdateDataAsync();

        CoverChanged?.Invoke(this, audio);
    }

    public async Task RemoveCoverAsync(Audio audio)
    {
        if (_meidaMap.Remove(audio.Id))
        {
            await UpdateDataAsync();

            CoverChanged?.Invoke(this, audio);
        }
    }

    private async Task UpdateDataAsync()
    {
        using var stream = new FileStream(_coverPath, FileMode.Create, FileAccess.Write, FileShare.ReadWrite, 4096, true);
        using var writer = new StreamWriter(stream);

        var sb = new StringBuilder();
        foreach (var item in _meidaMap)
        {
            sb.AppendLine($"{item.Key},{item.Value}");
        }

        await writer.WriteAsync(sb.ToString());
        await writer.FlushAsync();
        writer.Close();
    }
}