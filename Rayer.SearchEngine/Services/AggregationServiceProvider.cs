using Microsoft.Extensions.Options;
using Rayer.Core;
using Rayer.Core.Common;
using Rayer.Core.Framework.Injection;
using Rayer.SearchEngine.Core.Abstractions;
using Rayer.SearchEngine.Core.Abstractions.Provider;
using Rayer.SearchEngine.Core.Business.Login;
using Rayer.SearchEngine.Core.Business.Playlist;
using Rayer.SearchEngine.Core.Business.User;
using Rayer.SearchEngine.Core.Options;

namespace Rayer.SearchEngine.Services;

[Inject<IAggregationServiceProvider>]
internal class AggregationServiceProvider(IOptionsSnapshot<SearchEngineOptions> snapshot) : IAggregationServiceProvider
{
    private readonly SearchEngineOptions _searchEngineOptions = snapshot.Value;

    public ILoginManager LoginManager => AppCore.GetRequiredService<ILoginManager>();

    public IPlaylistService PlaylistService => AppCore.GetRequiredKeyedService<IPlaylistService>(SearcherType.Netease);

    public IUserService UserService => AppCore.GetRequiredKeyedService<IUserService>(SearcherType.Netease);

    public ISearchEngine SearchEngine => AppCore.GetRequiredKeyedService<ISearchEngine>(_searchEngineOptions.SearcherType);

    public ISearchAudioEngine AudioEngine => AppCore.GetRequiredKeyedService<ISearchAudioEngine>(_searchEngineOptions.SearcherType);
}