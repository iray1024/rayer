namespace Rayer.SearchEngine.Core.Options;

public class SearchEngineOptionsBuilder(SearchEngineOptions options)
{
    public SearchEngineOptionsBuilder SetHttpEndpoint(string httpEndpoint)
    {
        options.HttpEndpoint = httpEndpoint;

        return this;
    }

    public SearchEngineOptions Build()
    {
        return options;
    }
}