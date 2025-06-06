using Microsoft.Extensions.Options;
using Rayer.FrameworkCore.Injection;
using Rayer.SearchEngine.Core.Http.Abstractions;
using Rayer.SearchEngine.Core.Options;

namespace Rayer.SearchEngine.Netease.Http.Selector;

[Inject]
internal class TrackApiSelector(IOptionsSnapshot<SearchEngineOptions> snapshot) : ApiSelector(snapshot)
{
    public IParamBuilder TrackDetail()
    {
        return CreateBuilder(_searchEngineOptions.HttpEndpoint, ApiEndpoints.Track.TrackDetail);
    }

    public IParamBuilder TrackQualityDetail()
    {
        return CreateBuilder(_searchEngineOptions.HttpEndpoint, ApiEndpoints.Track.TrackQualityDetail);
    }

    public IParamBuilder GetTrack()
    {
        return CreateBuilder(_searchEngineOptions.HttpEndpoint, ApiEndpoints.Track.GetTrack);
    }
}