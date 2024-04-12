using Rayer.Core.Lyric.Abstractions;

namespace Rayer.SearchEngine.Lyric.Abstractions;

public interface ILyricSearchEngine
{
    Task<ISearchResult?> SearchAsync(ITrackMetadata track, SearcherType searcherType);

    Task<ISearchResult?> SearchAsync(ITrackMetadata track, SearcherType searcherType, MatchType matchType);

    Task<ILyricResult?> GetLyricAsync(ISearchResult tag);
}