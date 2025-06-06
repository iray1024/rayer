using Microsoft.Extensions.Options;
using Rayer.Core.Common;
using Rayer.FrameworkCore.Injection;
using Rayer.SearchEngine.Core.Abstractions;
using Rayer.SearchEngine.Core.Abstractions.Provider;
using Rayer.SearchEngine.Core.Domain.Aggregation;
using Rayer.SearchEngine.Core.Domain.Search;
using Rayer.SearchEngine.Core.Enums;
using Rayer.SearchEngine.Core.Options;
using Rayer.SearchEngine.Netease.Models.Search.Suggest;

namespace Rayer.SearchEngine.Netease.Engine;

[Inject<ISearchEngine>(ServiceKey = SearcherType.Netease)]
internal class SearchEngine : SearchEngineBase, ISearchEngine
{
    private readonly IAggregationServiceProvider _provider;
    private readonly SearchEngineOptions _searchEngineOptions;

    public SearchEngine(
        IOptionsSnapshot<SearchEngineOptions> snapshot,
        IAggregationServiceProvider provider)
    {
        _provider = provider;
        _searchEngineOptions = snapshot.Value;
    }

    public async Task<SearchAggregationModel> SearchAsync(string queryText, SearchType searchType, CancellationToken cancellationToken = default)
    {
        _searchEngineOptions.LatestQueryText = queryText;

        var model = new SearchAggregationModel(SearcherType.Netease);

        if (searchType is SearchType.Audio)
        {
            var audioResult = await _provider.AudioEngine.SearchAsync(queryText, 0);

            model.Audio = audioResult;
        }
        else if (searchType is SearchType.Album)
        {
            var albumResult = await _provider.AlbumEngine.SearchAsync(queryText, 0);

            model.Album = albumResult;
        }

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