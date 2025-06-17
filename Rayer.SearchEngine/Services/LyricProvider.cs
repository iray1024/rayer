using Rayer.Core.Abstractions;
using Rayer.Core.Common;
using Rayer.Core.Events;
using Rayer.Core.Framework.Settings.Abstractions;
using Rayer.Core.Lyric;
using Rayer.Core.Lyric.Abstractions;
using Rayer.Core.Lyric.Impl;
using Rayer.Core.Lyric.Models;
using Rayer.FrameworkCore.Injection;
using Rayer.SearchEngine.Lyric.Abstractions;

namespace Rayer.SearchEngine.Services;

[Inject<ILyricProvider>]
internal class LyricProvider : ILyricProvider
{
    private readonly ILyricSearchEngine _lyricSearchEngine;
    private readonly ILyricManager _lyricManager;
    private readonly IAudioManager _audioManager;
    private readonly ISettingsService _settingsService;

    public LyricProvider(
        ILyricSearchEngine lyricSearchEngine,
        ILyricManager lyricManager,
        IAudioManager audioManager,
        ISettingsService settingsService)
    {
        _lyricSearchEngine = lyricSearchEngine;
        _lyricManager = lyricManager;
        _audioManager = audioManager;
        _settingsService = settingsService;

        _audioManager.AudioPlaying += OnAudioPlaying;
        _audioManager.AudioChanged += OnAudioChanged;
        _audioManager.AudioPaused += OnAudioPaused;
        _audioManager.AudioStopped += OnAudioStopped;
    }

    public LyricData? LyricData { get; set; }

    public LyricSearcher LyricSearcher => _settingsService.Settings.LyricSearcher;

    public event EventHandler<AudioPlayingArgs>? AudioPlaying;
    public event EventHandler<AudioChangedArgs>? AudioChanged;
    public event EventHandler? AudioPaused;
    public event EventHandler? AudioStopped;
    public event EventHandler<SwitchLyricSearcherArgs>? LyricChanged;

    protected virtual void OnAudioPlaying(object? sender, AudioPlayingArgs e)
    {
        AudioPlaying?.Invoke(this, e);
    }

    protected virtual async void OnAudioChanged(object? sender, AudioChangedArgs e)
    {
        if (e.New.SearcherType is not SearcherType.Bilibili)
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
        else
        {
            LyricData = null;
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
            if (lyricResult is not null)
            {
                var (lyric, rawType) = lyricResult.GetLyricTarget();
                LyricData = LyricParser.ParseLyrics(lyric, rawType);

                if (LyricData is not null)
                {
                    var offset = _lyricManager.LoadOffset(_audioManager.Playback.Audio);
                    if (offset != 0)
                    {
                        foreach (var line in LyricData.Lines ?? [])
                        {
                            if (line is SyllableLineInfo syllable)
                            {
                                AdjustSyllableOffset(syllable, offset);
                            }
                            else
                            {
                                AdjustOffSet(line, offset);
                            }
                        }
                    }

                    return true;
                }
            }
        }

        LyricData = null;
        return false;
    }

    void ILyricProvider.FastForward()
    {
        if (LyricData is not null && LyricData.Lines is { Count: > 0 })
        {
            foreach (var line in LyricData.Lines)
            {
                if (line is SyllableLineInfo syllable)
                {
                    foreach (var item in syllable.Syllables)
                    {
                        AdjustSyllableLineInfo(item);
                    }

                    syllable.StartTime = syllable.Syllables.FirstOrDefault()?.StartTime;
                    syllable.EndTime = syllable.Syllables.LastOrDefault()?.EndTime;
                    syllable.Duration = TimeSpan.FromMilliseconds(syllable.EndTime.GetValueOrDefault() - syllable.StartTime.GetValueOrDefault());
                }
                else
                {
                    AdjustLineInfo(line);
                }
            }

            LyricChanged?.Invoke(this, new SwitchLyricSearcherArgs(false));
            _lyricManager.Store(_audioManager.Playback.Audio, -500);
        }

        static void AdjustLineInfo(ILineInfo line)
        {
            if (line.StartTime is int startTime)
            {
                line.StartTime = Math.Max(startTime - 500, 0);
            }

            line.EndTime -= 500;
        }

        static void AdjustSyllableLineInfo(ISyllableInfo line)
        {
            if (line.StartTime is int startTime)
            {
                line.StartTime = Math.Max(startTime - 500, 0);
            }

            line.EndTime -= 500;
        }
    }

    void ILyricProvider.FastBackward()
    {
        if (LyricData is not null && LyricData.Lines is { Count: > 0 })
        {
            foreach (var line in LyricData.Lines)
            {
                if (line is SyllableLineInfo syllable)
                {
                    foreach (var item in syllable.Syllables)
                    {
                        item.StartTime += 500;
                        item.EndTime += 500;
                    }

                    syllable.StartTime = syllable.Syllables.FirstOrDefault()?.StartTime;
                    syllable.EndTime = syllable.Syllables.LastOrDefault()?.EndTime;
                    syllable.Duration = TimeSpan.FromMilliseconds(syllable.EndTime.GetValueOrDefault() - syllable.StartTime.GetValueOrDefault());
                }
                else
                {
                    line.StartTime += 500;
                    line.EndTime += 500;
                }
            }

            LyricChanged?.Invoke(this, new SwitchLyricSearcherArgs(false));
            _lyricManager.Store(_audioManager.Playback.Audio, 500);
        }
    }

    private static void AdjustOffSet(ILineInfo line, int offset)
    {
        if (line.StartTime is int startTime)
        {
            line.StartTime = Math.Max(startTime + offset, 0);
        }

        line.EndTime += offset;
    }

    private static void AdjustSyllableOffset(SyllableLineInfo syllable, int offset)
    {
        foreach (var syllableItem in syllable.Syllables)
        {
            syllableItem.StartTime = Math.Max(syllableItem.StartTime + offset, 0);
            syllableItem.EndTime += offset;
        }

        syllable.StartTime = syllable.Syllables.FirstOrDefault()?.StartTime;
        syllable.EndTime = syllable.Syllables.LastOrDefault()?.EndTime;
        syllable.Duration = TimeSpan.FromMilliseconds(syllable.EndTime.GetValueOrDefault() - syllable.StartTime.GetValueOrDefault());
    }
}