using Rayer.SearchEngine.Core.Business.Login;
using Rayer.SearchEngine.Core.Business.Playlist;
using Rayer.SearchEngine.Core.Business.User;

namespace Rayer.SearchEngine.Core.Abstractions.Provider;

public interface IAggregationServiceProvider
{
    ILoginManager LoginManager { get; }

    IPlaylistService PlaylistService { get; }

    IUserService UserService { get; }

    ISearchEngine SearchEngine { get; }

    ISearchAudioEngine AudioEngine { get; }

    ISearchAlbumEngine AlbumEngine { get; }
}