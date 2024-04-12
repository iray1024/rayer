namespace Rayer.SearchEngine;

public class SearchEngineOptions
{
    public string HttpEndpoint { get; set; } = string.Empty;

    public IServiceProvider ServiceProvider { get; set; } = default!;
}