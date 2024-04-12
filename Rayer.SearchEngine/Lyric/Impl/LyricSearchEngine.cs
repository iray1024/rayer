using Rayer.Core.Lyric.Abstractions;
using Rayer.SearchEngine.Lyric.Abstractions;
using Rayer.SearchEngine.Lyric.Models;

namespace Rayer.SearchEngine.Lyric.Impl;

internal class LyricSearchEngine : ILyricSearchEngine
{
    private static readonly QQMusicSearcher _qqMusicSearcher = new();
    private static readonly NeteaseSearcher _neteaseSearcher = new();
    private static readonly KugouSearcher _kugouSearcher = new();

    public async Task<ISearchResult?> SearchAsync(ITrackMetadata track, SearcherType searcherType)
    {
        return searcherType switch
        {
            SearcherType.QQMusic => await _qqMusicSearcher.SearchForResult(track),
            SearcherType.Netease => await _neteaseSearcher.SearchForResult(track),
            SearcherType.Kugou => await _kugouSearcher.SearchForResult(track),
            _ => null,
        };
    }

    public async Task<ISearchResult?> SearchAsync(ITrackMetadata track, SearcherType searcherType, MatchType matchType)
    {
        return searcherType switch
        {
            SearcherType.QQMusic => await _qqMusicSearcher.SearchForResult(track, matchType),
            SearcherType.Netease => await _neteaseSearcher.SearchForResult(track, matchType),
            SearcherType.Kugou => await _kugouSearcher.SearchForResult(track, matchType),
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

        return null;
    }
}