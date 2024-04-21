using Rayer.SearchEngine.Core.Http.Abstractions;

namespace Rayer.SearchEngine.Netease.Http;

public class ParamBuilder(string httpEndpoint, string apiEndpoint) : IParamBuilder
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
        var url = _params.Count > 0
             ? $"{httpEndpoint}{apiEndpoint}?{Format(_params)}"
             : $"{httpEndpoint}{apiEndpoint}?{Format()}";

        return url;
    }

    private static string Format(Dictionary<string, string> paris)
    {
        var sb = new StringBuilder();

        foreach (var (key, value) in paris)
        {
            sb.Append($"{key}={value}&");
        }

        sb.Append(Format());

        return sb.ToString();
    }

    private static string Format()
    {
        return $"realIP={GetRealIP()}&timestamp={CreateTimestamp()}";
    }

    private static string CreateTimestamp()
    {
        return DateTime.Now.Ticks.ToString();
    }

    private static string GetRealIP()
    {
        return "192.168.1.2";
    }
}