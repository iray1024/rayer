using Rayer.Core.Common;

namespace Rayer.SearchEngine.Core.Abstractions.Provider;

public interface ISearchAudioEngineProvider : ISearchProvider
{
    ISearchAudioEngine AudioEngine { get; }

    ISearchAudioEngine GetAudioEngine(SearcherType searcherType);
}