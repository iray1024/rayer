﻿using Rayer.Core.Http;
using Rayer.FrameworkCore.Injection;
using Rayer.SearchEngine.Core.Http.Abstractions;
using Rayer.SearchEngine.Core.Http.Serialization;
using System.ComponentModel;
using System.IO;
using System.Numerics;
using System.Security.Cryptography;

namespace Rayer.SearchEngine.Lyric.Providers.Web.Netease;

[Inject]
internal class Api : RequestBase
{
    protected override string HttpRefer => "https://music.163.com/";

    private const string MODULUS = "00e0b509f6259df8642dbc35662901477df22677ec152b5ff68ace615bb7b725152b3ab17a876aea8a5aa76d2e417629ec4ee341f56135fccf695280104e0312ecbda92557c93870114af6c9d05c4f7f0c3685b7a46bee255932575cce10b424d813cfe4875d3e82047b97ddef52741d546b8e289dc6935b3ece0462db0a22b8e7";
    private const string NONCE = "0CoJUm6Qyw8W8jud";
    private const string PUBKEY = "010001";
    private const string VI = "0102030405060708";

    private readonly string _secretKey;
    private readonly string _encSecKey;

    public Api(IHttpClientProvider httpClientProvider)
        : base(httpClientProvider)
    {
        _secretKey = CreateSecretKey(16);
        _encSecKey = RSAEncode(_secretKey);
    }

    public enum SearchTypeEnum
    {
        [Description("单曲")] SONG_ID = 0,
        [Description("专辑")] ALBUM_ID = 1,
        [Description("歌单")] PLAYLIST_ID = 2,
    }

    public async Task<SearchResult?> Search(string keyword, SearchTypeEnum searchType)
    {
        // 1: 单曲, 10: 专辑, 100: 歌手, 1000: 歌单, 1002: 用户, 1004: MV, 1006: 歌词, 1009: 电台, 1014: 视频, 1018:综合, 2000:声音
        var type = searchType switch
        {
            SearchTypeEnum.SONG_ID => "1",
            SearchTypeEnum.ALBUM_ID => "10",
            SearchTypeEnum.PLAYLIST_ID => "1000",
            _ => "1",
        };

        var url = $"http://music.163.com/api/search/get/web?csrf_token=hlpretag=&hlposttag=&s={Uri.EscapeDataString(keyword)}&type={type}&offset=0&total=true&limit=20";

        var res = await GetAsync(url);

        return res.ToEntity<SearchResult>();
    }

    public async Task<Dictionary<long, Datum>> GetDatum(string[] songId, long bitrate = 999000)
    {
        var result = new Dictionary<long, Datum>();

        var urls = await GetSongsUrl(songId, bitrate);
        if (urls?.Code == 200)
        {
            foreach (var datum in urls.Data)
            {
                result.Add(datum.Id, datum);
            }
        }

        return result;
    }

    public async Task<Dictionary<long, Song>> GetSongs(string[] songIds)
    {
        var result = new Dictionary<long, Song>();

        if (songIds == null || songIds.Length < 1)
        {
            return result;
        }

        var detailResult = await GetDetail(songIds);
        if (detailResult == null || detailResult.Code != 200)
        {
            return result;
        }

        foreach (var song in detailResult.Songs)
        {
            result[song.Id] = song;
        }

        return result;
    }

    public async Task<AlbumResult?> GetAlbum(string albumId)
    {
        var url = $"https://music.163.com/weapi/v1/album/{albumId}?csrf_token=";

        var data = new Dictionary<string, string>
        {
            { "csrf_token", string.Empty },
        };

        var raw = await PostAsJsonAsync(url, Prepare(JsonSerializer.Serialize(data)));

        return JsonSerializer.Deserialize<AlbumResult>(raw);
    }

    public async Task<PlaylistResult?> GetPlaylist(string playlistId)
    {
        var url = $"https://music.163.com/weapi/v6/playlist/detail?csrf_token=";

        var data = new Dictionary<string, string>
        {
            { "csrf_token", string.Empty },
            { "id", playlistId },
            { "offset", "0" },
            { "total", "true" },
            { "limit", "1000" },
            { "n", "1000" }
        };

        var raw = await PostAsJsonAsync(url, Prepare(JsonSerializer.Serialize(data)));

        return JsonSerializer.Deserialize<PlaylistResult>(raw);
    }

    /// <summary>
    /// 获得原始歌词结果
    /// </summary>
    /// <param name="songId">音乐ID</param>
    /// <exception cref="WebException"></exception>
    /// <returns>一个
    /// <see cref="LyricResult"/></returns>
    public async Task<LyricResult?> GetLyric(string songId)
    {
        const string url = "https://music.163.com/weapi/song/lyric?csrf_token=";

        var data = new Dictionary<string, string>
        {
            { "id", songId },
            { "os", "pc" },
            { "lv", "-1" },
            { "kv", "-1" },
            { "tv", "-1" },
            { "rv", "-1" },
            { "csrf_token", string.Empty }
        };

        var raw = await PostAsJsonAsync(url, Prepare(JsonSerializer.Serialize(data)));

        return JsonSerializer.Deserialize<LyricResult>(raw);
    }

    /// <summary>
    /// 获得新版歌词结果（含逐字）
    /// </summary>
    /// <param name="songId">音乐ID</param>
    /// <exception cref="WebException"></exception>
    /// <returns>一个
    /// <see cref="LyricResult"/></returns>
    public async Task<LyricResult?> GetLyricNew(string songId)
    {
        const string url = "https://interface3.music.163.com/eapi/song/lyric/v1";

        var data = new Dictionary<string, string>
        {
            { "id", songId },
            { "cp", "false" },
            { "lv", "0" },
            { "kv", "0" },
            { "tv", "0" },
            { "rv", "0" },
            { "yv", "0" },
            { "ytv", "0" },
            { "yrv", "0" },
            { "csrf_token", string.Empty }
        };

        var raw = await EapiHelper.PostAsync(url, HttpClient, data);

        return raw.ToEntity<LyricResult>();
    }

    private async Task<SongUrls?> GetSongsUrl(string[] songId, long bitrate = 999000)
    {
        const string url = "https://music.163.com/weapi/song/enhance/player/url?csrf_token=";

        var data = new Dictionary<string, string>
        {
            { "ids", $"[{string.Join(",", songId)}]" },
            { "br", bitrate.ToString() },
            { "csrf_token", string.Empty }
        };

        var raw = await PostAsJsonAsync(url, Prepare(JsonSerializer.Serialize(data)));

        return JsonSerializer.Deserialize<SongUrls>(raw);
    }

    /// <summary>
    /// 批量获得歌曲详情
    /// </summary>
    /// <param name="songIds">歌曲ID</param>
    /// <exception cref="WebException"></exception>
    /// <returns></returns>
    private async Task<DetailResult?> GetDetail(IEnumerable<string> songIds)
    {
        try
        {
            const string url = "https://music.163.com/weapi/v3/song/detail?csrf_token=";

            var songRequests = new StringBuilder();
            foreach (var songId in songIds)
            {
                songRequests.Append("{'id':'").Append(songId).Append("'}").Append(',');
            }

            var data = new Dictionary<string, string>
            {
                {
                    "c",
                    "[" + songRequests.Remove(songRequests.Length - 1, 1) + "]"
                },
                { "csrf_token", string.Empty },
            };

            var raw = await PostAsJsonAsync(url, Prepare(JsonSerializer.Serialize(data)));

            return JsonSerializer.Deserialize<DetailResult>(raw);
        }
        catch
        {
            return null;
        }
    }

    private Dictionary<string, string> Prepare(string raw)
    {
        var data = new Dictionary<string, string>
        {
            ["params"] = AESEncode(AESEncode(raw, NONCE), _secretKey),
            ["encSecKey"] = _encSecKey,
        };
        return data;
    }

    private static string RSAEncode(string text)
    {
        var srtext = new string(text.Reverse().ToArray());
        var a = BCHexDec(BitConverter.ToString(Encoding.Default.GetBytes(srtext)).Replace("-", string.Empty));
        var b = BCHexDec(PUBKEY);
        var c = BCHexDec(MODULUS);
        var key = BigInteger.ModPow(a, b, c).ToString("x");
        key = key.PadLeft(256, '0');

        return key.Length > 256 ? key.Substring(key.Length - 256, 256) : key;
    }

    private static BigInteger BCHexDec(string hex)
    {
        var dec = new BigInteger(0);
        var len = hex.Length;

        for (var i = 0; i < len; i++)
        {
            dec += BigInteger.Multiply(new BigInteger(Convert.ToInt32(hex[i].ToString(), 16)),
                BigInteger.Pow(new BigInteger(16), len - i - 1));
        }

        return dec;
    }

    private static string AESEncode(string secretData, string secret = "TA3YiYCfY2dDJQgg")
    {
        byte[] encrypted;
        var IV = Encoding.UTF8.GetBytes(VI);

        using (var aes = Aes.Create())
        {
            aes.Key = Encoding.UTF8.GetBytes(secret);
            aes.IV = IV;
            aes.Mode = CipherMode.CBC;
            using var encryptor = aes.CreateEncryptor();
            using var stream = new MemoryStream();
            using var cstream = new CryptoStream(stream, encryptor, CryptoStreamMode.Write);
            using (var sw = new StreamWriter(cstream))
            {
                sw.Write(secretData);
            }

            encrypted = stream.ToArray();
        }

        return Convert.ToBase64String(encrypted);
    }

    private static string CreateSecretKey(int length)
    {
        const string str = "0123456789abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ";
        var sb = new StringBuilder(length);
        var rnd = new Random();

        for (var i = 0; i < length; ++i)
        {
            sb.Append(str[rnd.Next(0, str.Length)]);
        }

        return sb.ToString();
    }
}