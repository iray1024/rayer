using Rayer.SearchEngine.Core.Http.Abstractions;

namespace Rayer.SearchEngine.Bilibili.Http;

#pragma warning disable CS9113 // 参数未读。
internal class ParamBuilder(string httpEndpoint, string apiEndpoint) : IParamBuilder
#pragma warning restore CS9113 // 参数未读。
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