namespace Rayer.SearchEngine.Core.Http.Abstractions;

public interface IParamBuilder
{
    IParamBuilder WithParam(string name, string value);
    IParamBuilder WithParams(Dictionary<string, string> @params);

    string Build();
}