namespace Rayer.SearchEngine.Abstractions;

public class SearchEngineOptionsBuilder
{
    private readonly SearchEngineOptions _options = new();

    public SearchEngineOptionsBuilder SetHttpEndpoint(string httpEndpoint)
    {
        _options.HttpEndpoint = httpEndpoint;

        return this;
    }

    public SearchEngineOptions Build()
    {
        return _options;
    }
}