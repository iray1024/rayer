using CommunityToolkit.Mvvm.ComponentModel;
using Rayer.Core.Abstractions;
using Rayer.Core.Common;
using Rayer.Core.Framework.Injection;
using Rayer.Core.Framework.Settings.Abstractions;
using Rayer.Core.Models;
using Rayer.Core.Utils;
using Rayer.SearchEngine.Business.Search.Abstractions;
using Rayer.SearchEngine.Controls;
using Rayer.SearchEngine.Models.Response.Search;
using System.Windows;

namespace Rayer.SearchEngine.ViewModels.Presenter;

[Inject]
public partial class SearchAudioPresenterViewModel : ObservableObject, IPresenterViewModel<SearchAudioDetailResponse>
{
    private readonly ISearchAudioEngine _audioEngine;
    private readonly IAudioManager _audioManager;
    private readonly ISettingsService _settingsService;

    [ObservableProperty]
    private SearchAudioDetailResponse _presenterDataContext = null!;

    [ObservableProperty]
    private double _nameMaxWidth = 150;

    [ObservableProperty]
    private double _artistsNameMaxWidth = 250;

    [ObservableProperty]
    private double _albumNameMaxHeight = 250;

    [ObservableProperty]
    private double _durationMaxHeight = 35;

    [ObservableProperty]
    private Thickness _itemMargin = new (0,0,28,0);

    public SearchAudioPresenterViewModel(
        ISearchAudioEngine audioEngine,
        IAudioManager audioManager,
        ISettingsService settingsService)
    {
        _audioEngine = audioEngine;
        _audioManager = audioManager;
        _settingsService = settingsService;
    }

    public async Task PlayWebAudio(SearchAudioDetailAudioDetail item)
    {
        var audioInfomation = await _audioEngine.GetAudioAsync(item.Id);

        var audio = new Audio
        {
            Title = item.Name,
            Artists = item.Artists.Select(x => x.Name).ToArray(),
            Album = item.Album?.Name ?? string.Empty,
            Cover = item.Album?.Picture is not null ? ImageSourceUtils.Create(item.Album.Picture) : null,
            Duration = TimeSpan.FromMilliseconds(item.Duration),
            Path = audioInfomation.Data.FirstOrDefault()?.Url ?? string.Empty
        };

        if (_settingsService.Settings.PlaySingleAudioStrategy is PlaySingleAudioStrategy.AddToQueue)
        {
            var index = _audioManager.Playback.Queue.IndexOf(audio);

            if (index == -1)
            {
                _audioManager.Playback.Queue.Add(audio);
            }
        }
        else
        {
            //_audioManager.Playback.Queue.Clear();

        }

        await _audioManager.Playback.Play(audio);
    }
}