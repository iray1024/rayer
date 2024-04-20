using Rayer.Core.Framework.Injection;
using Rayer.SearchEngine.Abstractions;
using Rayer.SearchEngine.Abstractions.Provider;
using Rayer.SearchEngine.Extensions;
using Rayer.SearchEngine.Internal.Abstractions;
using Rayer.SearchEngine.Models.Response.Netease.Search;

namespace Rayer.SearchEngine.Services;

[Inject<ISearchEngine>]
internal class SearchEngine : SearchEngineBase, ISearchEngine
{
    private readonly ISearchAudioEngineProvider _audioEngineprovider;
    private readonly SearchEngineOptions _searchEngineOptions;

    public SearchEngine(
        IServiceProvider serviceProvider,
        SearchEngineOptions searchEngineOptions,
        ISearchAudioEngineProvider audioEngineprovider) : base(serviceProvider)
    {
        _audioEngineprovider = audioEngineprovider;
        _searchEngineOptions = searchEngineOptions;
    }

    public async Task<SearchAggregationModel> SearchAsync(string queryText, CancellationToken cancellationToken = default)
    {
        _searchEngineOptions.LatestQueryText = queryText;

        var model = new SearchAggregationModel();

        var audioResult = await _audioEngineprovider.GetAudioEngine().SearchAsync(queryText, 0);

        model.Audio = audioResult;

        return model;
    }

    public async Task<SearchSuggest> SuggestAsync(string keywords, CancellationToken cancellationToken = default)
    {
        var result = await Searcher.GetAsync(
            Search.SearchSuggestion()
                .WithParam("keywords", keywords)
                .Build());

        var response = result.ToEntity<SearchSuggest>();

        return response is not null ? response : default!;
    }
}