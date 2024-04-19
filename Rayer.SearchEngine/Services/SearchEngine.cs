using Rayer.Core.Framework.Injection;
using Rayer.SearchEngine.Abstractions;
using Rayer.SearchEngine.Business.Search.Abstractions;
using Rayer.SearchEngine.Extensions;
using Rayer.SearchEngine.Internal.Abstractions;
using Rayer.SearchEngine.Models.Response.Search;

namespace Rayer.SearchEngine.Services;

[Inject<ISearchEngine>]
internal class SearchEngine : SearchEngineBase, ISearchEngine
{
    private readonly ISearchAudioEngine _audioEngine;

    public SearchEngine(
        IServiceProvider serviceProvider,
        ISearchAudioEngine audioEngine) : base(serviceProvider)
    {
        _audioEngine = audioEngine;
    }

    public async Task<SearchAggregationModel> SearchAsync(string queryText, CancellationToken cancellationToken = default)
    {
        var model = new SearchAggregationModel();

        var audioResult = await _audioEngine.SearchAsync(queryText, 0);

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