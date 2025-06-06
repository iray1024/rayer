using Microsoft.Extensions.Options;
using Rayer.FrameworkCore.Injection;
using Rayer.SearchEngine.Core.Http.Abstractions;
using Rayer.SearchEngine.Core.Options;

namespace Rayer.SearchEngine.Netease.Http.Selector;

[Inject]
internal class LyricApiSelector(IOptionsSnapshot<SearchEngineOptions> snapshot) : ApiSelector(snapshot)
{
    public IParamBuilder GetLyric()
    {
        return CreateBuilder(_searchEngineOptions.HttpEndpoint, ApiEndpoints.Lyric.GetLyric);
    }

    public IParamBuilder GetLyricEx()
    {
        return CreateBuilder(_searchEngineOptions.HttpEndpoint, ApiEndpoints.Lyric.GetLyricEx);
    }
}