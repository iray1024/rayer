using Rayer.Core.Abstractions;
using Rayer.Core.Common;
using Rayer.Core.Framework.Injection;
using Rayer.Core.Framework.Settings.Abstractions;
using Rayer.Core.Lyric;
using Rayer.Core.Lyric.Abstractions;
using Rayer.Core.Lyric.Impl;
using Rayer.Core.Lyric.Models;
using Rayer.SearchEngine.Abstractions;
using Rayer.SearchEngine.Events;
using Rayer.SearchEngine.Lyric.Abstractions;

namespace Rayer.SearchEngine.Services;

[Inject<ILyricProvider>]
internal class LyricProvider : ILyricProvider
{
    private readonly ILyricSearchEngine _lyricSearchEngine;
    private readonly IAudioManager _audioManager;
    private readonly ISettingsService _settingsService;

    public LyricProvider(
        ILyricSearchEngine lyricSearchEngine,
        IAudioManager audioManager,
        ISettingsService settingsService)
    {
        _lyricSearchEngine = lyricSearchEngine;
        _audioManager = audioManager;
        _settingsService = settingsService;

        _audioManager.AudioPlaying += OnAudioPlaying;
        _audioManager.AudioChanged += OnAudioChanged;
        _audioManager.AudioPaused += OnAudioPaused;
        _audioManager.AudioStopped += OnAudioStopped;
    }

    public LyricData? LyricData { get; set; }

    public LyricSearcher LyricSearcher => _settingsService.Settings.LyricSearcher;

    public event EventHandler<Core.Events.AudioPlayingArgs>? AudioPlaying;
    public event EventHandler<Core.Events.AudioChangedArgs>? AudioChanged;
    public event EventHandler? AudioPaused;
    public event EventHandler? AudioStopped;
    public event EventHandler<SwitchLyricSearcherArgs>? LyricChanged;

    protected virtual void OnAudioPlaying(object? sender, Core.Events.AudioPlayingArgs e)
    {
        AudioPlaying?.Invoke(this, e);
    }

    protected virtual async void OnAudioChanged(object? sender, Core.Events.AudioChangedArgs e)
    {
        var metadata = new TrackMultiArtistMetadata()
        {
            Title = e.New.Title,
            Album = e.New.Album,
            Artists = [.. e.New.Artists],
            DurationMs = (int)e.New.Duration.TotalMilliseconds
        };

        var result = await InternalSearchAsync(metadata);

        if (result)
        {
            AudioChanged?.Invoke(this, e);
        }
    }

    protected virtual void OnAudioPaused(object? sender, EventArgs e)
    {
        AudioPaused?.Invoke(this, EventArgs.Empty);
    }

    protected virtual void OnAudioStopped(object? sender, EventArgs e)
    {
        AudioStopped?.Invoke(this, EventArgs.Empty);
    }

    public async Task SwitchSearcherAsync()
    {
        var audio = _audioManager.Playback.Audio;

        var metadata = new TrackMultiArtistMetadata()
        {
            Title = audio.Title,
            Album = audio.Album,
            Artists = [.. audio.Artists],
            DurationMs = (int)audio.Duration.TotalMilliseconds
        };

        var result = await InternalSearchAsync(metadata);

        if (result)
        {
            LyricChanged?.Invoke(this, SwitchLyricSearcherArgs.True);
        }
        else
        {
            LyricChanged?.Invoke(this, SwitchLyricSearcherArgs.False);
        }
    }

    private async Task<bool> InternalSearchAsync(ITrackMetadata metadata)
    {
        var result = await _lyricSearchEngine.SearchAsync(metadata, _settingsService.Settings.LyricSearcher);

        if (result is not null)
        {
            var lyricResult = await _lyricSearchEngine.GetLyricAsync(result);

            if (lyricResult is not null && lyricResult.GetLyric().Lyric is string lyric)
            {
                LyricData = LyricParser.ParseLyrics(lyric, Core.Lyric.Enums.LyricRawType.Lrc);

                if (LyricData is not null)
                {
                    return true;
                }
            }
        }

        LyricData = null;
        return false;
    }
}