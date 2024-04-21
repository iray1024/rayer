using Rayer.SearchEngine.Core.Abstractions;

namespace Rayer.SearchEngine.Abstractions.Provider;

public interface ISearchAudioEngineProvider
{
    ISearchAudioEngine AudioEngine { get; }
}