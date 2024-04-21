namespace Rayer.SearchEngine.Core.Http.Abstractions;

public abstract class ApiSelectorBase
{
    protected abstract IParamBuilder CreateBuilder(string httpEndpoint, string apiEndpoint);
}