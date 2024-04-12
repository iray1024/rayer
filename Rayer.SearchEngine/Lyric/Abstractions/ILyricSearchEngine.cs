using Rayer.Core.Common;
using Rayer.Core.Lyric.Abstractions;

namespace Rayer.SearchEngine.Lyric.Abstractions;

public interface ILyricSearchEngine
{
    Task<ISearchResult?> SearchAsync(ITrackMetadata track, LyricSearcher searcherType);

    Task<ISearchResult?> SearchAsync(ITrackMetadata track, LyricSearcher searcherType, MatchType matchType);

    Task<ILyricResult?> GetLyricAsync(ISearchResult tag);
}