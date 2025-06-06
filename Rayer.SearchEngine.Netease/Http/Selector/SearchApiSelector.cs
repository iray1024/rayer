﻿using Microsoft.Extensions.Options;
using Rayer.FrameworkCore.Injection;
using Rayer.SearchEngine.Core.Http.Abstractions;
using Rayer.SearchEngine.Core.Options;

namespace Rayer.SearchEngine.Netease.Http.Selector;

[Inject]
internal class SearchApiSelector(IOptionsSnapshot<SearchEngineOptions> snapshot) : ApiSelector(snapshot)
{
    public IParamBuilder SampleSearch()
    {
        return CreateBuilder(_searchEngineOptions.HttpEndpoint, ApiEndpoints.Search.SampleSearch);
    }

    public IParamBuilder SearchSuggestion()
    {
        return CreateBuilder(_searchEngineOptions.HttpEndpoint, ApiEndpoints.Search.SearchSuggestion);
    }
}