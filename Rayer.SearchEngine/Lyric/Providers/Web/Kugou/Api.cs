using Lyricify.Lyrics.Providers.Web.Kugou;
using Rayer.Core.Abstractions;
using Rayer.SearchEngine.Internal.Abstractions;

namespace Rayer.SearchEngine.Lyric.Providers.Web.Kugou;

internal class Api(IHttpClientProvider httpClientProvider) : RequestBase(httpClientProvider)
{
    public async Task<SearchSongResponse?> GetSearchSong(string keywords)
    {
        var response = await GetAsync($"http://mobilecdn.kugou.com/api/v3/search/song?format=json&keyword={keywords}&page=1&pagesize=20&showtype=1");

        var resp = JsonSerializer.Deserialize<SearchSongResponse>(response);

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
        var resp = JsonSerializer.Deserialize<SearchLyricsResponse>(response);

        return resp;
    }
}