using Rayer.Core.Framework.Injection;
using Rayer.SearchEngine.Core.Http.Abstractions;
using Rayer.SearchEngine.Core.Options;

namespace Rayer.SearchEngine.Netease.Http.Selector;

[Inject]
internal class LyricApiSelector(SearchEngineOptions options) : ApiSelector
{
    public IParamBuilder GetLyric()
    {
        return CreateBuilder(options.HttpEndpoint, ApiEndpoints.Lyric.GetLyric);
    }

    public IParamBuilder GetLyricEx()
    {
        return CreateBuilder(options.HttpEndpoint, ApiEndpoints.Lyric.GetLyricEx);
    }
}