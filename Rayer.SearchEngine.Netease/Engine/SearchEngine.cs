using Rayer.Core.Framework.Injection;
using Rayer.SearchEngine.Abstractions.Provider;
using Rayer.SearchEngine.Core.Abstractions;
using Rayer.SearchEngine.Core.Domain.Aggregation;
using Rayer.SearchEngine.Core.Domain.Search;
using Rayer.SearchEngine.Core.Enums;
using Rayer.SearchEngine.Core.Options;
using Rayer.SearchEngine.Netease.Models.Search;

namespace Rayer.SearchEngine.Netease.Engine;

[Inject<ISearchEngine>(ServiceKey = SearcherType.Netease)]
internal class SearchEngine : SearchEngineBase, ISearchEngine
{
    private readonly ISearchAudioEngineProvider _audioEngineProvider;
    private readonly SearchEngineOptions _searchEngineOptions;

    public SearchEngine(
        SearchEngineOptions searchEngineOptions,
        ISearchAudioEngineProvider audioEngineProvider)
    {
        _audioEngineProvider = audioEngineProvider;
        _searchEngineOptions = searchEngineOptions;
    }

    public async Task<SearchAggregationModel> SearchAsync(string queryText, CancellationToken cancellationToken = default)
    {
        _searchEngineOptions.LatestQueryText = queryText;

        var model = new SearchAggregationModel();

        var audioResult = await _audioEngineProvider.AudioEngine.SearchAsync(queryText, 0);

        model.Audio = audioResult;

        return model;
    }

    public async Task<SearchSuggest> SuggestAsync(string keywords, CancellationToken cancellationToken = default)
    {
        var result = await Searcher.GetAsync(
            SearchSelector.SearchSuggestion()
                .WithParam("keywords", keywords)
                .Build());

        var response = result.ToEntity<SearchSuggestModel>();

        if (response is not null)
        {
            var domain = Mapper.Map<SearchSuggest>(response);

            return domain;
        }

        return default!;
    }
}