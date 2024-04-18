using CommunityToolkit.Mvvm.ComponentModel;
using Rayer.Core;
using Rayer.Core.Framework.Injection;
using Rayer.Core.Lyric;
using Rayer.SearchEngine.Abstractions;
using Rayer.SearchEngine.Business.Data.Abstractions;
using Rayer.SearchEngine.Business.Login.Abstractions;
using Rayer.SearchEngine.Business.Lyric.Abstractions;
using Rayer.SearchEngine.Business.Playlist.Abstractions;
using Rayer.SearchEngine.Business.User.Abstractions;
using Rayer.SearchEngine.Extensions;
using Rayer.SearchEngine.Models.Domian;
using Rayer.SearchEngine.Models.Response.Login.User;
using Rayer.SearchEngine.Models.Response.User;
using Rayer.SearchEngine.Views.Windows;
using System.Windows;
using System.Windows.Threading;
using Wpf.Ui;

namespace Rayer.SearchEngine.ViewModels.Explore;

[Inject<IExploreLibraryDataProvider>]
public partial class ExploreLibraryViewModel : ObservableObject, IExploreLibraryDataProvider
{
    private readonly ILoginManager _loginManager;

    private readonly IUserService _userService;
    private readonly IPlaylistService _playlistService;

    [ObservableProperty]
    private AccountInfoResponse _account = default!;

    [ObservableProperty]
    private ExploreLibraryModel _model;

    private string _title = string.Empty;

    public ExploreLibraryViewModel(
        ILoginManager loginManager,
        IUserService userService,
        IPlaylistService playlistService)
    {
        _loginManager = loginManager;
        _userService = userService;
        _playlistService = playlistService;

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
        await _loginManager.RefreshLoginStateAsync(AppCore.StoppingToken);
        var accountInfo = await _loginManager.GetAccountInfoAsync(AppCore.StoppingToken);

        if (accountInfo.Profile is not null)
        {
            Account = accountInfo;

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
        var accountInfo = await _loginManager.GetAccountInfoAsync(AppCore.StoppingToken);

        if (accountInfo is { Account.AnonimousUser: false } && accountInfo.Profile is not null)
        {
            Account = accountInfo;

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

    partial void OnAccountChanged(AccountInfoResponse value)
    {
        if (value is { Profile: not null })
        {
            Title = $"{value.Profile.NickName}的音乐库";
        }
    }

    private async Task LoadInformationAsync()
    {
        var likelist = await _userService.GetLikelistAsync(Account.Account.Id);

        var userPlaylists = await _userService.GetPlaylistAsync(Account.Account.Id);

        if (userPlaylists.Playlist.Length > 0)
        {
            var playlistDetail = await _playlistService.GetPlaylistDetailAsync(userPlaylists.Playlist[0].Id);

            Model.LikeCount = userPlaylists.Playlist[0].TrackCount;
            Model.TotalLikeAudios = MapToAudioDetail(playlistDetail, playlistDetail.Playlist.TrackCount);
            Model.PainedLikeAudios = Model.TotalLikeAudios[..12];

            var randomAudio = playlistDetail.Playlist.Tracks[Random.Shared.Next(0, Model.LikeCount - 1)];
            Model.RandomLyrics = await GetRandomLyricsAsync(randomAudio.Id, randomAudio.Name);

            Model.Detail.Playlist = userPlaylists.Playlist.Length > 1 ? userPlaylists.Playlist[1..] : userPlaylists.Playlist;

            OnPropertyChanged(nameof(Model));

            Loaded?.Invoke(this, EventArgs.Empty);

            Timer.Tick += OnTick;
            Timer.Start();
        }
    }

    private async void OnTick(object? sender, EventArgs e)
    {
        var randomAudio = Model.TotalLikeAudios[Random.Shared.Next(0, Model.LikeCount - 1)];
        Model.RandomLyrics = await GetRandomLyricsAsync(randomAudio.Id, randomAudio.Name);

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

        var lyricData = LyricParser.ParseLyrics(lyricResult.Lrc.Lyric, Core.Lyric.Enums.LyricRawType.Lrc);

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

    private static AudioDetail[] MapToAudioDetail(PlaylistDetailResponse response, int count)
    {
        var tracks = response.Playlist.Tracks;
        var privileges = response.Privileges;

        var audioDetails = new AudioDetail[count];

        for (var i = 0; i < count; i++)
        {
            var audio = audioDetails[i] = new AudioDetail();
            var track = tracks[i];
            var privilege = privileges[i];

            audio.Id = track.Id;
            audio.Name = track.Name;
            audio.Artists = track.Artists;
            audio.Album = track.Album;
            audio.Pop = track.Pop;
            audio.Fee = track.Fee;
            audio.Duration = track.Duration;
            audio.OriginCoverType = track.OriginCoverType;
            audio.NoCopyright = track.NoCopyright;
            audio.Privilege = privilege;

            audio.Playable();
        }

        return audioDetails;
    }
}