using Rayer.SearchEngine.Core.Enums;

namespace Rayer.SearchEngine.Core.Options;

public class SearchEngineOptions
{
    public string HttpEndpoint { get; set; } = string.Empty;

    public IServiceProvider ServiceProvider { get; set; } = default!;

    public SearcherType SearcherType { get; set; } = SearcherType.Netease;

    public string LatestQueryText { get; set; } = string.Empty;
}