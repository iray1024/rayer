using CommunityToolkit.Mvvm.ComponentModel;
using Microsoft.Extensions.Options;
using Rayer.Core;
using Rayer.Core.Common;
using Rayer.Core.Framework;
using Rayer.Core.Framework.Injection;
using Rayer.SearchEngine.Abstractions;
using Rayer.SearchEngine.Core.Abstractions.Provider;
using Rayer.SearchEngine.Core.Enums;
using Rayer.SearchEngine.Core.Options;
using System.Windows;
using System.Windows.Threading;

namespace Rayer.SearchEngine.ViewModels.Explore;

[Inject]
public partial class SearchTitleBarViewModel : ObservableObject
{
    private readonly SearchEngineOptions _searchEngineOptions;

    [ObservableProperty]
    private SearcherType _searcher = 0;

    private bool _isInitialized = false;
    private CancellationTokenSource _requestToken = new();

    public SearchTitleBarViewModel()
    {
        _searchEngineOptions = AppCore.GetRequiredService<IOptionsSnapshot<SearchEngineOptions>>().Value;

        _searcher = _searchEngineOptions.SearcherType;
    }

    public SearchType SearchType { get; set; }

    public async Task OnSearcherChanged()
    {
        if (!_isInitialized)
        {
            _isInitialized = true;

            return;
        }

        _searchEngineOptions.SearcherType = Searcher;

        var provider = AppCore.GetRequiredService<ISearchEngineProvider>();
        var loader = AppCore.GetRequiredService<ILoaderProvider>();

        loader.Loading();

        await _requestToken.CancelAsync();
        _requestToken = new CancellationTokenSource();

        var model = await Task.Run(() =>
            provider.SearchEngine.SearchAsync(_searchEngineOptions.LatestQueryText, SearchType, AppCore.StoppingToken),
            AppCore.StoppingToken);

        Application.Current.Dispatcher.Invoke(() =>
        {
            model.QueryText = _searchEngineOptions.LatestQueryText;

            var searchAware = AppCore.GetRequiredService<ISearchAware>();

            searchAware.OnSearch(model);

            loader.Loaded();
        },
        DispatcherPriority.Normal,
        _requestToken.Token);
    }
}