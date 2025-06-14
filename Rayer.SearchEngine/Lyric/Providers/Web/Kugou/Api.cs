﻿using Rayer.Core.Http;
using Rayer.FrameworkCore.Injection;
using Rayer.SearchEngine.Core.Http.Abstractions;
using Rayer.SearchEngine.Core.Http.Serialization;
using Rayer.SearchEngine.Lyric.Decrypter.Krc;

namespace Rayer.SearchEngine.Lyric.Providers.Web.Kugou;

[Inject]
internal class Api(IHttpClientProvider httpClientProvider) : RequestBase(httpClientProvider)
{
    public async Task<SearchSongResponse?> GetSearchSong(string keywords)
    {
        var response = await GetAsync($"http://mobilecdn.kugou.com/api/v3/search/song?format=json&keyword={keywords}&page=1&pagesize=20&showtype=1");

        var resp = response.ToEntity<SearchSongResponse>();

        return resp;
    }

    public async Task<SearchLyricsResponse?> GetSearchLyrics(string? keywords = null, int? duration = null, string? hash = null)
    {
        var durationPara = string.Empty;
        if (duration != null)
        {
            durationPara = $"&duration={duration}";
        }

        hash ??= string.Empty;

        var response = await GetAsync($"https://lyrics.kugou.com/search?ver=1&man=yes&client=pc&keyword={keywords}{durationPara}&hash={hash}");
        var resp = response.ToEntity<SearchLyricsResponse>();

        return resp;
    }

    public async Task<LyricResult?> GetLyricAsync(string id, string accessKey)
    {
        var krcResult = await Helper.GetLyricsAsync(id, accessKey);
        if (!string.IsNullOrWhiteSpace(krcResult))
        {
            return new LyricResult
            {
                KrcContent = krcResult
            };
        }

        var response = await GetAsync($"http://lyrics.kugou.com/download?ver=1&client=pc&id={id}&accesskey={accessKey}&fmt=lrc&charset=utf8");

        var resp = response.ToEntity<LyricResult>();

        return resp;
    }
}