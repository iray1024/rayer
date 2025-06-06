﻿using Microsoft.Extensions.Options;
using Rayer.Core.Common;
using Rayer.FrameworkCore;
using Rayer.FrameworkCore.Injection;
using Rayer.SearchEngine.Core.Abstractions;
using Rayer.SearchEngine.Core.Abstractions.Provider;
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

    SearcherType ISearchProvider.CurrentSearcher => _searchEngineOptions.SearcherType;

    ISearchAudioEngine ISearchAudioEngineProvider.GetAudioEngine(SearcherType searcherType)
    {
        return AppCore.GetRequiredKeyedService<ISearchAudioEngine>(searcherType);
    }

    private ISearchAudioEngine GetAudioEngine()
    {
        return AppCore.GetRequiredKeyedService<ISearchAudioEngine>(_searchEngineOptions.SearcherType);
    }
}