using Rayer.Core.Framework.Injection;
using Rayer.SearchEngine.Core.Http.Abstractions;
using Rayer.SearchEngine.Core.Options;

namespace Rayer.SearchEngine.Netease.Http.Selector;

[Inject]
internal class TrackApiSelector(SearchEngineOptions options) : ApiSelector
{
    public IParamBuilder TrackDetail()
    {
        return CreateBuilder(options.HttpEndpoint, ApiEndpoints.Track.TrackDetail);
    }

    public IParamBuilder TrackQualityDetail()
    {
        return CreateBuilder(options.HttpEndpoint, ApiEndpoints.Track.TrackQualityDetail);
    }

    public IParamBuilder GetTrack()
    {
        return CreateBuilder(options.HttpEndpoint, ApiEndpoints.Track.GetTrack);
    }
}