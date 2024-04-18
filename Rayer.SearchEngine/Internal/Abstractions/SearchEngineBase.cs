using Microsoft.Extensions.DependencyInjection;
using Rayer.SearchEngine.Internal.ApiSelector;

namespace Rayer.SearchEngine.Internal.Abstractions;

internal abstract class SearchEngineBase
{
    protected SearchEngineBase(IServiceProvider serviceProvider)
    {
        ServiceProvider = serviceProvider;

        Searcher = serviceProvider.GetRequiredService<IRequestService>();
        Login = serviceProvider.GetRequiredService<LoginApiSelector>();
        Account = serviceProvider.GetRequiredService<AccountApiSelector>();
        User = serviceProvider.GetRequiredService<UserApiSelector>();
        Playlist = serviceProvider.GetRequiredService<PlaylistApiSelector>();
        Search = serviceProvider.GetRequiredService<SearchApiSelector>();
        Track = serviceProvider.GetRequiredService<TrackApiSelector>();
        Lyric = serviceProvider.GetRequiredService<LyricApiSelector>();
    }

    protected IServiceProvider ServiceProvider { get; }

    protected IRequestService Searcher { get; }

    protected LoginApiSelector Login { get; }
    protected AccountApiSelector Account { get; }
    protected UserApiSelector User { get; }
    protected PlaylistApiSelector Playlist { get; }
    protected SearchApiSelector Search { get; }
    protected TrackApiSelector Track { get; }
    protected LyricApiSelector Lyric { get; }
}