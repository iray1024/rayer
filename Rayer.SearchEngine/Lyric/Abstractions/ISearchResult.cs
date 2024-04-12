using Rayer.SearchEngine.Lyric.Searchers.Abstractions;

namespace Rayer.SearchEngine.Lyric.Abstractions;

public interface ISearchResult
{
    public ISearcher Searcher { get; }

    /// <summary>
    /// 曲目名
    /// </summary>
    public string Title { get; }

    /// <summary>
    /// 艺人列表
    /// </summary>
    public string[] Artists { get; }

    /// <summary>
    /// 艺人名
    /// </summary>
    public string Artist => string.Join(", ", Artists);

    /// <summary>
    /// 专辑
    /// </summary>
    public string Album { get; }

    /// <summary>
    /// 专辑艺人列表
    /// </summary>
    public string[]? AlbumArtists { get; }

    /// <summary>
    /// 专辑艺人名
    /// </summary>
    public string? AlbumArtist => string.Join(", ", AlbumArtists ?? new string[0]);

    /// <summary>
    /// 曲目时长
    /// </summary>
    public int? DurationMs { get; }

    /// <summary>
    /// 匹配程度
    /// </summary>
    public MatchType? MatchType { get; protected set; }

    /// <summary>
    /// 设置匹配程度
    /// </summary>
    /// <param name="matchType"></param>
    internal void SetMatchType(MatchType? matchType)
    {
        MatchType = matchType;
    }
}