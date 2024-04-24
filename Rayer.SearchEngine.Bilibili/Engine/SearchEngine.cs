using Microsoft.Extensions.Options;
using Rayer.Core.Common;
using Rayer.Core.Framework.Injection;
using Rayer.SearchEngine.Core.Abstractions;
using Rayer.SearchEngine.Core.Abstractions.Provider;
using Rayer.SearchEngine.Core.Domain.Aggregation;
using Rayer.SearchEngine.Core.Domain.Search;
using Rayer.SearchEngine.Core.Enums;
using Rayer.SearchEngine.Core.Options;

namespace Rayer.SearchEngine.Bilibili.Engine;

[Inject<ISearchEngine>(ServiceKey = SearcherType.Bilibili)]
internal class SearchEngine : SearchEngineBase, ISearchEngine
{
    private readonly ISearchAudioEngineProvider _audioEngineProvider;
    private readonly SearchEngineOptions _searchEngineOptions;

    public SearchEngine(
        ISearchAudioEngineProvider audioEngineProvider,
        IOptionsSnapshot<SearchEngineOptions> snapshot)
    {
        _audioEngineProvider = audioEngineProvider;
        _searchEngineOptions = snapshot.Value;
    }

    public async Task<SearchAggregationModel> SearchAsync(string queryText, SearchType searchType, CancellationToken cancellationToken = default)
    {
        _searchEngineOptions.LatestQueryText = queryText;

        var model = new SearchAggregationModel(SearcherType.Bilibili);

        var audioResult = await _audioEngineProvider.AudioEngine.SearchAsync(queryText, 0);

        model.Audio = audioResult;

        return model;
    }

    public Task<SearchSuggest> SuggestAsync(string keywords, CancellationToken cancellationToken = default)
    {
        return Task.FromResult(SearchSuggest.Empty);
    }
}