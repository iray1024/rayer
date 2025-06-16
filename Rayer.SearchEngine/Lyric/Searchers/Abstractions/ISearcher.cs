using Rayer.Core.Lyric.Abstractions;
using Rayer.SearchEngine.Lyric.Abstractions;

namespace Rayer.SearchEngine.Lyric.Searchers.Abstractions;

public interface ISearcher
{
    public string Name { get; }

    /// <summary>
    /// 搜索源显示名称 (in English)
    /// </summary>
    public string DisplayName { get; }

    /// <summary>
    /// 搜索最佳匹配的曲目
    /// </summary>
    public Task<ISearchResult?> SearchForResult(ITrackMetadata track);

    /// <summary>
    /// 搜索最佳匹配的曲目
    /// </summary>
    /// <param name="track"></param>
    /// <param name="minimumMatch">最低匹配要求</param>
    public Task<ISearchResult?> SearchForResult(ITrackMetadata track, MatchType minimumMatch);

    /// <summary>
    /// 搜索匹配的曲目列表
    /// </summary>
    public Task<List<ISearchResult>> SearchForResults(ITrackMetadata track);

    /// <summary>
    /// 搜索匹配的曲目列表
    /// </summary>
    /// <param name="track"></param>
    /// <param name="fullSearch">是否是完整搜索</param>
    /// <returns></returns>
    public Task<List<ISearchResult>> SearchForResults(ITrackMetadata track, bool fullSearch);

    /// <summary>
    /// 搜索关键字的曲目列表
    /// </summary>
    public Task<List<ISearchResult>?> SearchForResults(string searchString);
}