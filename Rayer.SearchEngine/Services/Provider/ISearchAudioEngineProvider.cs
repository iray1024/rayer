using Rayer.Core.Framework.Injection;
using Rayer.SearchEngine.Abstractions.Provider;
using Rayer.SearchEngine.Business.Search.Abstractions;
using Rayer.SearchEngine.Enums;

namespace Rayer.SearchEngine.Services.Provider;

[Inject<ISearchAudioEngineProvider>]
internal class SearchAudioEngineProvider : ISearchAudioEngineProvider
{
    private readonly SearchEngineOptions _searchEngineOptions;

    public SearchAudioEngineProvider(SearchEngineOptions searchEngineOptions)
    {
        _searchEngineOptions = searchEngineOptions;
    }

    public ISearchAudioEngine GetAudioEngine()
    {
        switch (_searchEngineOptions.SearcherType)
        {
            case SearcherType.Netease:
                break;
            case SearcherType.Bilibili:
                break;
            default:
                break;
        }
    }
}