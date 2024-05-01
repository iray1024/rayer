using CommunityToolkit.Mvvm.ComponentModel;
using Rayer.Core.Abstractions;
using Rayer.Core.Common;
using Rayer.Core.Controls;
using Rayer.Core.Framework.Injection;
using Rayer.Core.Framework.Settings.Abstractions;
using Rayer.Core.Menu;
using Rayer.Core.Models;
using Rayer.Core.Utils;
using Rayer.SearchEngine.Controls;
using Rayer.SearchEngine.Core.Abstractions.Provider;
using Rayer.SearchEngine.Core.Domain.Aduio;
using System.Windows.Controls;

namespace Rayer.SearchEngine.ViewModels.Presenter;

[Inject]
public partial class SearchAudioPresenterViewModel : AdaptiveViewModelBase, IPresenterViewModel<SearchAudio>
{
    private readonly ISearchAudioEngineProvider _audioEngineProvider;
    private readonly IAudioManager _audioManager;
    private readonly ISettingsService _settingsService;

    [ObservableProperty]
    private SearchAudio _presenterDataContext = null!;

    public SearchAudioPresenterViewModel(
        ISearchAudioEngineProvider audioEngineProvider,
        IAudioManager audioManager,
        ISettingsService settingsService,
        IContextMenuFactory contextMenuFactory)
    {
        _audioEngineProvider = audioEngineProvider;
        _audioManager = audioManager;
        _settingsService = settingsService;

        ContextMenu = contextMenuFactory.CreateContextMenu(ContextMenuScope.Library);
    }

    public ContextMenu ContextMenu { get; }

    public event EventHandler? DataChanged;

    public async Task PlayWebAudio(SearchAudioDetail item)
    {
        var webAudio = await _audioEngineProvider.AudioEngine.GetAudioAsync(item);

        if (!_audioManager.Playback.TryGetAudio(item.Id, out var existsAudio))
        {
            var audio = new Audio()
            {
                Id = item.Id,
                Title = item.Title,
                Artists = item.Artists.Select(x => x.Name).ToArray(),
                Album = item.Album?.Title ?? string.Empty,
                Cover = item.Album?.Cover is not null ? ImageSourceFactory.Create(item.Album.Cover) : null,
                Duration = item.Duration,
                Path = webAudio.Url ?? string.Empty,
                IsVirualWebSource = true,
                SearcherType = _audioEngineProvider.CurrentSearcher
            };

            // 后续需要实现同时加入所有搜索项进入播放队列时，去除true
            if (true || _settingsService.Settings.PlaySingleAudioStrategy is PlaySingleAudioStrategy.AddToQueue)
            {
                _audioManager.Playback.Queue.Add(audio);
            }

            await _audioManager.Playback.Play(audio);
        }
        else
        {
            await _audioManager.Playback.Play(existsAudio);
        }
    }

    partial void OnPresenterDataContextChanged(SearchAudio value)
    {
        DataChanged?.Invoke(this, EventArgs.Empty);
    }
}