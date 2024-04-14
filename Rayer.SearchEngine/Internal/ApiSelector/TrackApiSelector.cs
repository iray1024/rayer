using Rayer.Core.Framework.Injection;
using Rayer.SearchEngine.Internal.Abstractions;

namespace Rayer.SearchEngine.Internal.ApiSelector;

[Inject]
internal class TrackApiSelector(SearchEngineOptions options) : ApiSelectorBase(options)
{
    public ParamBuilder TrackDetail()
    {
        return CreateBuilder(ApiEndpoints.Track.TrackDetail);
    }

    public ParamBuilder TrackQualityDetail()
    {
        return CreateBuilder(ApiEndpoints.Track.TrackQualityDetail);
    }

    public ParamBuilder GetTrack()
    {
        return CreateBuilder(ApiEndpoints.Track.GetTrack);
    }
}