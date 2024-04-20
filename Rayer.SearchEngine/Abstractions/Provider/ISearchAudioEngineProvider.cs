using Rayer.SearchEngine.Business.Search.Abstractions;

namespace Rayer.SearchEngine.Abstractions.Provider;

public interface ISearchAudioEngineProvider
{
    ISearchAudioEngine GetAudioEngine();
}