using CommunityToolkit.Mvvm.ComponentModel;
using Microsoft.Extensions.Caching.Memory;
using Rayer.Core;
using Rayer.Core.Framework;
using Rayer.Core.Framework.Injection;
using Rayer.Core.Lyric;
using Rayer.SearchEngine.Core.Abstractions.Provider;
using Rayer.SearchEngine.Core.Business.Data;
using Rayer.SearchEngine.Core.Business.Lyric;
using Rayer.SearchEngine.Core.Domain.Aggregation;
using Rayer.SearchEngine.Core.Domain.Authority;
using Rayer.SearchEngine.Core.Domain.Playlist;
using Rayer.SearchEngine.Views.Windows;
using System.Windows;
using System.Windows.Threading;
using Wpf.Ui;

namespace Rayer.SearchEngine.ViewModels.Explore;

[Inject<IExploreLibraryDataProvider>(ResolveServiceType = true)]
public partial class ExploreLibraryViewModel : ObservableObject, IExploreLibraryDataProvider
{
    private readonly IAggregationServiceProvider _provider;
    private readonly IMemoryCache _cache;

    [ObservableProperty]
    private User _user = default!;

    [ObservableProperty]
    private ExploreLibraryModel _model;

    private string _title = string.Empty;

    public ExploreLibraryViewModel(
        IAggregationServiceProvider provider,
        IMemoryCache cache)
    {
        _provider = provider;
        _cache = cache;

        Model = new();

        Timer = new DispatcherTimer(DispatcherPriority.Normal, Application.Current.Dispatcher)
        {
            Interval = TimeSpan.FromSeconds(15)
        };

    }

    public string Title
    {
        get => _title;
        set
        {
            _title = value;
            OnPropertyChanged(nameof(Title));
        }
    }

    public DispatcherTimer Timer { get; }

    public event EventHandler? LoginSucceed;
    public event EventHandler? Loaded;

    public async Task OnLoadAsync()
    {
        await _provider.LoginManager.RefreshLoginStateAsync(AppCore.StoppingToken);
        var user = await _provider.LoginManager.GetAccountInfoAsync(AppCore.StoppingToken);

        if (user.Profile is not null)
        {
            User = user;

            if (AppCore.GetRequiredService<ILoaderProvider>() is { IsLoading: true } loader)
            {
                await LoadInformationAsync();

                loader.Loaded();

                LoginSucceed?.Invoke(this, EventArgs.Empty);
            }
        }
        else
        {
            var loginWindow = AppCore.GetRequiredService<LoginWindow>();

            loginWindow.Owner = AppCore.MainWindow;
            loginWindow.Closed += OnLoginWindowClosed;

            loginWindow.Show();
        }
    }

    public void Unload()
    {
        Timer.Stop();
        Timer.Tick -= OnTick;
    }

    private async void OnLoginWindowClosed(object? sender, EventArgs e)
    {
        var user = await _provider.LoginManager.GetAccountInfoAsync(AppCore.StoppingToken);

        if (user is { Account.Anonimous: false } && user.Profile is not null)
        {
            User = user;

            await LoadInformationAsync();

            LoginSucceed?.Invoke(this, EventArgs.Empty);
        }
        else
        {
            AppCore.GetRequiredService<INavigationService>().Navigate("本地音乐");
        }

        if (AppCore.GetRequiredService<ILoaderProvider>() is { IsLoading: true } loader)
        {
            loader.Loaded();
        }
    }

    partial void OnUserChanged(User value)
    {
        if (value is { Profile: not null })
        {
            Title = $"{value.Profile.Name}的音乐库";
        }
    }

    private async Task LoadInformationAsync()
    {
        var likelist = await _provider.UserService.GetLikelistAsync(User.Account.Id);

        var userPlaylists = await _provider.UserService.GetPlaylistAsync(User.Account.Id);
        var userFavAlbums = await _provider.UserService.GetFavAlbumsAsync(User.Account.Id);

        if (userPlaylists.Length > 0)
        {
            var cacheKey = userPlaylists[0].GetHashCode();

            if (!_cache.TryGetValue<PlaylistDetail>(cacheKey, out var userLikelistDetail) || userLikelistDetail is null)
            {
                userLikelistDetail = await _provider.PlaylistService.GetPlaylistDetailAsync(userPlaylists[0].Id);

                _cache.Set(cacheKey, userLikelistDetail, TimeSpan.FromMinutes(1));
            }

            Model.LikeCount = userPlaylists[0].AudioCount;
            Model.FavoriteList = userLikelistDetail;
            Model.PainedLikeAudios = Model.FavoriteList.Audios[..12];

            var randomAudio = userLikelistDetail.Audios[Random.Shared.Next(0, Model.LikeCount - 1)];
            Model.RandomLyrics = await GetRandomLyricsAsync(randomAudio.Id, randomAudio.Title);
        }

        if (userFavAlbums.Length > 0)
        {
            Model.Detail.FavAlbum = userFavAlbums;
        }

        Model.Detail.Playlist = userPlaylists;

        OnPropertyChanged(nameof(Model));

        Loaded?.Invoke(this, EventArgs.Empty);

        Timer.Tick += OnTick;
        Timer.Start();
    }

    private async void OnTick(object? sender, EventArgs e)
    {
        var randomAudio = Model.FavoriteList.Audios[Random.Shared.Next(0, Model.LikeCount - 1)];
        Model.RandomLyrics = await GetRandomLyricsAsync(randomAudio.Id, randomAudio.Title);

        OnPropertyChanged(nameof(Model));
    }

    private async Task<string[]> GetRandomLyricsAsync(long id, string name)
    {
        var engine = AppCore.GetRequiredService<ISpecialLyricService>();

        var lyricResult = await engine.GetLyricAsync(id);

        if (lyricResult.Lrc.Lyric.Contains("纯音乐"))
        {
            return Model.RandomLyrics;
        }

        var lyricData = LyricParser.ParseLyrics(lyricResult.Lrc.Lyric, Rayer.Core.Lyric.Enums.LyricRawType.Lrc);

        if (lyricData is { Lines: not null })
        {
            var lyric = lyricData.Lines
                .Skip(10)
                .Where(x =>
                    !string.IsNullOrEmpty(x.Text) &&
                    !string.IsNullOrWhiteSpace(x.Text) &&
                    !x.Text.Contains("作词") &&
                    !x.Text.Contains("作曲") &&
                    !x.Text.Contains("编曲"))
                .ToArray();

            var startIndex = Random.Shared.Next(0, lyric.Length - 12);

            var result = lyric
                .Skip(startIndex)
                .Take(3)
                .Select(x => x.Text)
                .ToList();

            result.Insert(0, name);

            return [.. result];
        }

        return Model.RandomLyrics;
    }
}