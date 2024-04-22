using Rayer.SearchEngine.Core.Http.Abstractions;

namespace Rayer.SearchEngine.Bilibili.Http;

internal class ParamBuilder(string httpEndpoint, string apiEndpoint) : IParamBuilder
{
    private Dictionary<string, string> _params = [];

    public IParamBuilder WithParam(string name, string value)
    {
        _params.TryAdd(name, value);

        return this;
    }

    public IParamBuilder WithParams(Dictionary<string, string> @params)
    {
        _params = @params;

        return this;
    }

    public string Build()
    {
        throw new NotImplementedException();
    }
}