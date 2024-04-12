using Rayer.Core.Abstractions;
using Rayer.Core.Lyric;
using Rayer.Core.Lyric.Data;
using Rayer.Core.Lyric.Impl;
using Rayer.SearchEngine.Abstractions;
using Rayer.SearchEngine.Lyric.Abstractions;

namespace Rayer.SearchEngine.Services;

internal class LyricProvider : ILyricProvider
{
    private readonly ILyricSearchEngine _lyricSearchEngine;
    private readonly IAudioManager _audioManager;

    public LyricProvider(ILyricSearchEngine lyricSearchEngine, IAudioManager audioManager)
    {
        _lyricSearchEngine = lyricSearchEngine;
        _audioManager = audioManager;

        _audioManager.AudioPlaying += OnAudioPlaying;
        _audioManager.AudioChanged += OnAudioChanged;
        _audioManager.AudioPaused += OnAudioPaused;
        _audioManager.AudioStopped += OnAudioStopped;
    }

    public LyricData? LyricData { get; set; }

    public event EventHandler<Core.Events.AudioPlayingArgs>? AudioPlaying;
    public event EventHandler? AudioChanged;
    public event EventHandler? AudioPaused;
    public event EventHandler? AudioStopped;

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

        var result = await _lyricSearchEngine.SearchAsync(metadata, SearcherType.Netease);

        if (result is not null)
        {
            var lyricResult = await _lyricSearchEngine.GetLyricAsync(result);

            if (lyricResult is not null && lyricResult.GetLyric().Lyric is string lyric)
            {
                LyricData = LyricParser.ParseLyrics(lyric, Core.Lyric.Enums.LyricRawType.Lrc);

                if (LyricData is not null)
                {
                    AudioChanged?.Invoke(this, EventArgs.Empty);
                }
            }
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
}