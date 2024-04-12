using Rayer.Core.Common;
using Rayer.Core.Lyric.Abstractions;
using Rayer.SearchEngine.Lyric.Abstractions;
using Rayer.SearchEngine.Lyric.Models;
using Rayer.SearchEngine.Lyric.Searchers.Impl;

namespace Rayer.SearchEngine.Lyric.Impl;

internal class LyricSearchEngine : ILyricSearchEngine
{
    private static readonly QQMusicSearcher _qqMusicSearcher = new();
    private static readonly NeteaseSearcher _neteaseSearcher = new();
    private static readonly KugouSearcher _kugouSearcher = new();

    public async Task<ISearchResult?> SearchAsync(ITrackMetadata track, LyricSearcher searcherType)
    {
        return searcherType switch
        {
            LyricSearcher.QQMusic => await _qqMusicSearcher.SearchForResult(track),
            LyricSearcher.Netease => await _neteaseSearcher.SearchForResult(track),
            LyricSearcher.Kugou => await _kugouSearcher.SearchForResult(track),
            _ => null,
        };
    }

    public async Task<ISearchResult?> SearchAsync(ITrackMetadata track, LyricSearcher searcherType, MatchType matchType)
    {
        return searcherType switch
        {
            LyricSearcher.QQMusic => await _qqMusicSearcher.SearchForResult(track, matchType),
            LyricSearcher.Netease => await _neteaseSearcher.SearchForResult(track, matchType),
            LyricSearcher.Kugou => await _kugouSearcher.SearchForResult(track, matchType),
            _ => null,
        };
    }

    public async Task<ILyricResult?> GetLyricAsync(ISearchResult tag)
    {
        if (tag is QQMusicSearchResult qq)
        {
            var qqResult = await Providers.Web.Providers.QQMusicApi.GetLyric(qq.Mid);

            return qqResult;
        }
        else if (tag is NeteaseSearchResult netease)
        {
            var neteaseResult = await Providers.Web.Providers.NeteaseApi.GetLyricNew(netease.Id.ToString());

            return neteaseResult;
        }
        else if (tag is KugouSearchResult kugou)
        {
            var kugouSearchResult = await Providers.Web.Providers.KugouApi.GetSearchLyrics(null, kugou.DurationMs, kugou.Hash);

            if (kugouSearchResult is not null && kugouSearchResult.Candidates is { Count: > 0 })
            {
                var candidate = kugouSearchResult.Candidates[0];

                var kugouResult = await Providers.Web.Providers.KugouApi.GetLyricAsync(candidate.Id, candidate.AccessKey);

                return kugouResult;
            }
        }

        return null;
    }
}