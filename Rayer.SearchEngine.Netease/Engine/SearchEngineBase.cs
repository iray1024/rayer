using Microsoft.Extensions.DependencyInjection;
using Rayer.Core.Common;
using Rayer.SearchEngine.Core.Abstractions;
using Rayer.SearchEngine.Netease.Http.Selector;

namespace Rayer.SearchEngine.Netease.Engine;

internal abstract class SearchEngineBase : SearchEngineCoreBase
{
    protected SearchEngineBase()
        : base(SearcherType.Netease)
    {
        LoginSelector = ServiceProvider.GetRequiredService<LoginApiSelector>();
        AccountSelector = ServiceProvider.GetRequiredService<AccountApiSelector>();
        UserSelector = ServiceProvider.GetRequiredService<UserApiSelector>();
        PlaylistSelector = ServiceProvider.GetRequiredService<PlaylistApiSelector>();
        AlbumSelector = ServiceProvider.GetRequiredService<AlbumApiSelector>();
        SearchSelector = ServiceProvider.GetRequiredService<SearchApiSelector>();
        TrackSelector = ServiceProvider.GetRequiredService<TrackApiSelector>();
        LyricSelector = ServiceProvider.GetRequiredService<LyricApiSelector>();
    }

    protected LoginApiSelector LoginSelector { get; }
    protected AccountApiSelector AccountSelector { get; }
    protected UserApiSelector UserSelector { get; }
    protected PlaylistApiSelector PlaylistSelector { get; }
    protected AlbumApiSelector AlbumSelector { get; }
    protected SearchApiSelector SearchSelector { get; }
    protected TrackApiSelector TrackSelector { get; }
    protected LyricApiSelector LyricSelector { get; }
}