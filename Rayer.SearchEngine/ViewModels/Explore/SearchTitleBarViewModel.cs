using CommunityToolkit.Mvvm.ComponentModel;
using Rayer.Core;
using Rayer.Core.Framework.Injection;
using Rayer.SearchEngine.Abstractions;
using Rayer.SearchEngine.Core.Abstractions.Provider;
using Rayer.SearchEngine.Core.Enums;
using Rayer.SearchEngine.Core.Options;

namespace Rayer.SearchEngine.ViewModels.Explore;

[Inject]
public partial class SearchTitleBarViewModel : ObservableObject
{
    [ObservableProperty]
    private SearcherType _searcher = 0;

    private bool _isInitialized = false;

    public SearchTitleBarViewModel()
    {

    }

    public async Task OnSearcherChanged()
    {
        if (!_isInitialized)
        {
            _isInitialized = true;

            return;
        }

        var searchEngineOptions = AppCore.GetRequiredService<SearchEngineOptions>();

        searchEngineOptions.SearcherType = Searcher;

        var provider = AppCore.GetRequiredService<ISearchEngineProvider>();

        var model = await provider.SearchEngine.SearchAsync(searchEngineOptions.LatestQueryText, AppCore.StoppingToken);

        model.QueryText = searchEngineOptions.LatestQueryText;

        var searchAware = AppCore.GetRequiredService<ISearchAware>();

        searchAware.OnSearch(model);
    }
}