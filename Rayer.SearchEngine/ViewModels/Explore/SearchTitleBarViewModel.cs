using CommunityToolkit.Mvvm.ComponentModel;
using Rayer.Core;
using Rayer.Core.Framework.Injection;
using Rayer.SearchEngine.Abstractions;
using Rayer.SearchEngine.Enums;

namespace Rayer.SearchEngine.ViewModels.Explore;

[Inject]
public partial class SearchTitleBarViewModel : ObservableObject
{
    [ObservableProperty]
    private SearcherType _searcher = 0;

    public SearchTitleBarViewModel()
    {

    }

    public async Task OnSearcherChanged()
    {
        var searchEngineOptions = AppCore.GetRequiredService<SearchEngineOptions>();

        searchEngineOptions.SearcherType = Searcher;

        var engine = AppCore.GetRequiredService<ISearchEngine>();

        var model = await engine.SearchAsync(searchEngineOptions.LatestQueryText, AppCore.StoppingToken);

        var searchAware = AppCore.GetRequiredService<ISearchAware>();

        searchAware.OnSearch(model);
    }
}