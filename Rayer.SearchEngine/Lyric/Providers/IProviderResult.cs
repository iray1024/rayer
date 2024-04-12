using Rayer.SearchEngine.Lyric.Abstractions;

namespace Rayer.SearchEngine.Lyric.Providers;

/// <summary>
/// 歌词提供结果接口
/// </summary>
public interface IProviderResult
{
    /// <summary>
    /// 歌词提供者
    /// </summary>
    public IProvider Provider { get; }

    /// <summary>
    /// 搜索结果 (曲目)
    /// </summary>
    public ISearchResult SearchResult { get; }
}