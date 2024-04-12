namespace Rayer.SearchEngine.Internal.Abstractions;

internal abstract class ApiSelectorBase(SearchEngineOptions options)
{
    protected ParamBuilder CreateBuilder(string apiEndpoint)
    {
        return new ParamBuilder(options.HttpEndpoint, apiEndpoint);
    }
}