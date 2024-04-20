using Rayer.SearchEngine.Enums;

namespace Rayer.SearchEngine;

public class SearchEngineOptions
{
    public string HttpEndpoint { get; set; } = string.Empty;

    public IServiceProvider ServiceProvider { get; set; } = default!;

    public SearcherType SearcherType { get; set; } = SearcherType.Netease;

    public string LatestQueryText { get; set; } = string.Empty;
}