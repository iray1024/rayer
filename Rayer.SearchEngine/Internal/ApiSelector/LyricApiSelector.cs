using Rayer.Core.Framework.Injection;
using Rayer.SearchEngine.Internal.Abstractions;

namespace Rayer.SearchEngine.Internal.ApiSelector;

[Inject]
internal class LyricApiSelector(SearchEngineOptions options) : ApiSelectorBase(options)
{
    public ParamBuilder GetLyric()
    {
        return CreateBuilder(ApiEndpoints.Lyric.GetLyric);
    }

    public ParamBuilder GetLyricEx()
    {
        return CreateBuilder(ApiEndpoints.Lyric.GetLyricEx);
    }
}