using Microsoft.Extensions.Options;
using Rayer.Core;
using Rayer.Core.Framework.Injection;
using Rayer.SearchEngine.Abstractions.Provider;
using Rayer.SearchEngine.Core.Abstractions;
using Rayer.SearchEngine.Core.Options;

namespace Rayer.SearchEngine.Services.Provider;

[Inject<ISearchAudioEngineProvider>]
internal class SearchAudioEngineProvider : ISearchAudioEngineProvider
{
    private readonly SearchEngineOptions _searchEngineOptions;

    public SearchAudioEngineProvider(IOptionsSnapshot<SearchEngineOptions> snapshot)
    {
        _searchEngineOptions = snapshot.Value;
    }

    public ISearchAudioEngine AudioEngine => GetAudioEngine();

    private ISearchAudioEngine GetAudioEngine()
    {
        return AppCore.GetRequiredKeyedService<ISearchAudioEngine>(_searchEngineOptions.SearcherType);
    }
}