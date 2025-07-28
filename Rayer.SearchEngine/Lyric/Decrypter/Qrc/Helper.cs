using Rayer.SearchEngine.Lyric.Providers.Web.QQMusic;

namespace Rayer.SearchEngine.Lyric.Decrypter.Qrc;

public class Helper
{
    /// <summary>
    /// 通过 Mid 获取解密后的歌词
    /// </summary>
    /// <param name="mid">QQ 音乐歌曲 Mid</param>
    public static LyricResult? GetLyricsByMid(string mid)
    {
        var song = Providers.Web.Providers.QQMusicApi.GetSong(mid).Result;
        if (song == null || song.Data is not { Length: > 0 })
        {
            return null;
        }

        var id = song.Data?[0].Id;
        return Providers.Web.Providers.QQMusicApi.GetLyricsAsync(id.ToString()!).Result;
    }

    /// <summary>
    /// 通过 Mid 获取解密后的歌词
    /// </summary>
    /// <param name="mid">QQ 音乐歌曲 Mid</param>
    public static async Task<LyricResult?> GetLyricsByMidAsync(string mid)
    {
        var song = await Providers.Web.Providers.QQMusicApi.GetSong(mid);
        if (song == null || song.Data is not { Length: > 0 })
        {
            return null;
        }

        var id = song.Data?[0].Id;
        return await Providers.Web.Providers.QQMusicApi.GetLyricsAsync(id.ToString()!);
    }

    /// <summary>
    /// 通过 ID 获取解密后的歌词
    /// </summary>
    /// <param name="id">QQ 音乐歌曲 ID</param>
    public static LyricResult? GetLyrics(string id)
    {
        return Providers.Web.Providers.QQMusicApi.GetLyricsAsync(id).Result;
    }

    /// <summary>
    /// 通过 ID 获取解密后的歌词
    /// </summary>
    /// <param name="id">QQ 音乐歌曲 ID</param>
    public static async Task<LyricResult?> GetLyricsAsync(string id)
    {
        return await Providers.Web.Providers.QQMusicApi.GetLyricsAsync(id);
    }
}
