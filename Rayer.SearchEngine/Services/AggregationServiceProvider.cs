using Rayer.Core;
using Rayer.Core.Framework.Injection;
using Rayer.SearchEngine.Core.Abstractions;
using Rayer.SearchEngine.Core.Abstractions.Provider;
using Rayer.SearchEngine.Core.Business.Login;
using Rayer.SearchEngine.Core.Business.Playlist;
using Rayer.SearchEngine.Core.Business.User;
using Rayer.SearchEngine.Core.Options;

namespace Rayer.SearchEngine.Services;

[Inject<IAggregationServiceProvider>]
internal class AggregationServiceProvider(SearchEngineOptions searchEngineOptions) : IAggregationServiceProvider
{
    public ILoginManager LoginManager => AppCore.GetRequiredService<ILoginManager>();

    public IPlaylistService PlaylistService => AppCore.GetRequiredKeyedService<IPlaylistService>(searchEngineOptions.SearcherType);

    public IUserService UserService => AppCore.GetRequiredKeyedService<IUserService>(searchEngineOptions.SearcherType);

    public ISearchEngine SearchEngine => AppCore.GetRequiredKeyedService<ISearchEngine>(searchEngineOptions.SearcherType);

    public ISearchAudioEngine AudioEngine => AppCore.GetRequiredKeyedService<ISearchAudioEngine>(searchEngineOptions.SearcherType);
}