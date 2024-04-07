using Microsoft.Extensions.DependencyInjection;
using NAudio.Wave;
using Rayer.Core.Abstractions;
using Rayer.Core.Common;
using Rayer.Core.Events;
using Rayer.Core.Models;
using System.Windows;
using System.Windows.Threading;

namespace Rayer.Core.Playing;

public class Playback : IDisposable
{
    private WaveOutEvent? _device;
    private WaveMetadata _metadata = new();

    private readonly IPlayQueueProvider _playQueueProvider;
    private readonly IWaveMetadataFactory _metadataFactory;

    private readonly IAudioManager _audioManager = null!;

    private readonly SemaphoreSlim _semaphore = new(2, 2);

    private static readonly Audio _fallbackAudio = new();

    private static readonly TimeSpan _jumpThreshold = TimeSpan.FromSeconds(5);
    private static readonly TimeSpan _fadeOutThreshold = TimeSpan.FromMilliseconds(1000);

    private static readonly float _semitone = MathF.Pow(2, 1.0f / 12);
    private static readonly float _upOneTone = _semitone * _semitone;
    private static readonly float _downOneTone = 1.0f / _upOneTone;

    private bool _hasFadeOut = false;

    private float _volume = 1f;
    public float Volume
    {
        get => _volume;
        set
        {
            _volume = Math.Min(Math.Max(value, 0), 1);
            SetVolume();
        }
    }

    private float _pitch = 1f;
    public float Pitch
    {
        get => _pitch;
        set
        {
            _pitch = Math.Min(Math.Max(value, 0.5f), 2);
            SetPitch();
        }
    }

    public bool Repeat { get; set; } = true;

    public bool Shuffle { get; set; } = false;

    public TimeSpan CurrentTime
    {
        get => _metadata.Reader is not null ? _metadata.Reader.CurrentTime : TimeSpan.Zero;
        set
        {
            if (_metadata.Reader is not null)
            {
                _metadata.Reader.CurrentTime = new TimeSpan(Math.Min(value.Ticks, _metadata.Reader.TotalTime.Ticks));
            }
        }
    }

    public TimeSpan TotalTime => _metadata.Reader is not null ? _metadata.Reader.TotalTime : TimeSpan.Zero;

    public long Position
    {
        get => _metadata.Reader is not null ? _metadata.Reader.Position : 0;
        set
        {
            if (_metadata.Reader is not null)
            {
                _metadata.Reader.Position = Math.Min(value, _metadata.Reader.Length - 10000);
            }
        }
    }

    public long Length => _metadata.Reader is not null ? _metadata.Reader.Length : 0;

    public PlaybackState PlaybackState => _device is not null
        ? _device.PlaybackState
        : PlaybackState.Stopped;

    public Audio Audio { get; set; } = null!;

    public DispatcherTimer DispatcherTimer { get; } =
        new DispatcherTimer(DispatcherPriority.Render) { Interval = TimeSpan.FromMilliseconds(100) };

    public Playback(
        IAudioManager audioManager,
        IServiceProvider serviceProvider)
    {
        _audioManager = audioManager;

        _playQueueProvider = serviceProvider.GetRequiredService<IPlayQueueProvider>();
        _metadataFactory = serviceProvider.GetRequiredService<IWaveMetadataFactory>();

        Queue.AddRange(_audioManager.Audios);

        DispatcherTimer.Tick += OnTick;
    }

    public SortableObservableCollection<Audio> Queue => _playQueueProvider.Queue;

    public bool Playing { get; set; } = false;

    public event AudioPlayingEventHandler? AudioPlaying;
    public event EventHandler? AudioPaused;
    public event AudioChangedEventHandler? AudioChanged;
    public event EventHandler? AudioStopped;

    public void Initialize(float volume, float pitch, PlayloopMode playloopMode)
    {
        Volume = volume;
        Pitch = pitch;

        if (playloopMode is PlayloopMode.List)
        {
            Repeat = false;
            Shuffle = false;
        }
        else if (playloopMode is PlayloopMode.Single)
        {
            Repeat = true;
            Shuffle = false;
        }
        else
        {
            Repeat = false;
            Shuffle = true;
        }
    }

    public void Seek(double value)
    {
        if (_metadata.Reader is not null)
        {
            _metadata.Reader.Position = (long)(_metadata.Reader.Length * value * 0.01);

            if (_hasFadeOut)
            {
                _hasFadeOut = false;
                _metadata.FadeInOutSampleProvider?.BeginFadeIn(100);
            }
        }
    }

    public async Task Jump(bool negative = false)
    {
        if (_metadata.Reader is not null)
        {
            var threshold = (long)(_metadata.Reader.Length * (_jumpThreshold / _metadata.Reader.TotalTime));

            var targetPosition = _metadata.Reader.Position + (threshold * (negative ? -1 : 1));

            if (targetPosition < 0)
            {
                _metadata.Reader.Position = 0;
            }
            else if (targetPosition >= _metadata.Reader.Length)
            {
                await Next();
            }
            else
            {
                _metadata.Reader.Position = targetPosition;
            }

            if (_hasFadeOut)
            {
                _hasFadeOut = false;
                _metadata.FadeInOutSampleProvider?.BeginFadeIn(100);
            }
        }
    }

    public async Task PlayRepeat()
    {
        await Load(false);
        Resume();
    }

    public async Task Play()
    {
        await Load();
        Resume();
    }

    public async Task Play(Audio audio)
    {
        CheckAudio(ref audio);

        if (Audio != audio)
        {
            Audio = audio;
        }

        await Load();
        Resume();
    }

    public async Task Load(bool closePreDevice = true)
    {
        if (closePreDevice)
        {
            await _semaphore.WaitAsync();
            Stop();
        }

        await EnsureDeviceCreated();
        OpenFile();
    }

    public void Resume(bool fadeIn = true)
    {
        _hasFadeOut = false;
        if (_device is not null && _metadata.Reader is not null && _device.PlaybackState is not PlaybackState.Playing)
        {
            DispatcherTimer.Start();

            var oldState = _device.PlaybackState;

            _device.Play();
            if (fadeIn)
            {
                _metadata.FadeInOutSampleProvider?.BeginFadeIn(1000);
            }

            Playing = true;

            AudioPlaying?.Invoke(this, new AudioPlayingArgs() { PlaybackState = oldState });
        }
    }

    public void Pause()
    {
        DispatcherTimer.Stop();

        _device?.Pause();

        AudioPaused?.Invoke(this, EventArgs.Empty);
    }

    public void Stop()
    {
        DispatcherTimer.Stop();

        Playing = false;

        _device?.Stop();

        if (_metadata.Reader is not null)
        {
            CloseFile();
        }

        _device?.Dispose();
        _device = null;
    }

    public void StopPlay()
    {
        DispatcherTimer.Stop();

        Playing = false;

        _device?.Stop();

        if (_metadata.Reader is not null)
        {
            CloseFile();
        }

        _device?.Dispose();
        _device = null;

        Audio = _fallbackAudio;

        AudioStopped?.Invoke(this, EventArgs.Empty);
    }

    public async Task Next()
    {
        var index = GetNextAudioIndex();

        Audio = Queue[index];

        await Play(Audio);
    }

    public async Task Previous()
    {
        var index = Queue.IndexOf(Audio) - 1;

        index = index < 0 ? Queue.Count - 1 : index;

        var audio = Queue[index];

        if (Audio != audio)
        {
            Audio = audio;

            await Load();
        }

        Resume();
    }

    public void UpdateEqualizer()
    {
        _metadata.Equalizer?.Update();
    }

    private async Task EnsureDeviceCreated()
    {
        if (_device is null)
        {
            await _semaphore.WaitAsync();
            CreateDevice();
            _semaphore.Release();
        }
    }

    private void OpenFile()
    {
        try
        {
            _metadata = _metadataFactory.CreateWaveMetadata(Audio.Path);

            if (_metadata.PitchShiftingSampleProvider is not null)
            {
                _metadata.PitchShiftingSampleProvider.PitchFactor = Pitch;
            }

            _device?.Init(_metadata);

            AudioChanged?.Invoke(this, new AudioChangedArgs() { New = Audio });
        }
        catch
        {
            CloseFile();
        }
    }

    private void CloseFile()
    {
        _metadata.Dispose();
    }

    private void CreateDevice()
    {
        _device = new WaveOutEvent() { Volume = _volume, DesiredLatency = 200 };

        _device.PlaybackStopped += async (s, a) =>
        {
            await Task.Run(async () =>
            {
                await Application.Current.Dispatcher.InvokeAsync(async () =>
                {
                    if (_metadata.Reader is not null)
                    {
                        _semaphore.Release(2 - _semaphore.CurrentCount);

                        _metadata.Reader.Position = 0;

                        if (Repeat && Playing)
                        {
                            await PlayRepeat();
                        }
                        else
                        {
                            await Next();
                        }
                    }
                    else
                    {
                        _semaphore.Release();
                    }
                });
            });
        };
    }

    private void SetVolume()
    {
        if (_device is not null)
        {
            _device.Volume = _volume;
        }
    }

    private void SetPitch()
    {
        if (_metadata.PitchShiftingSampleProvider is not null)
        {
            _metadata.PitchShiftingSampleProvider.PitchFactor = MathF.Round(Pitch, 2, MidpointRounding.ToZero);
        }
    }

    private int GetNextAudioIndex()
    {
        var index = Shuffle
            ? Random.Shared.Next(0, Queue.Count)
            : Queue.IndexOf(Audio) + 1;

        return index >= Queue.Count ? 0 : index;
    }

    private void CheckAudio(ref Audio audio)
    {
        if (Queue.Count == 0)
        {
            return;
        }

        audio ??= Queue[0];
    }

    private async void OnTick(object? sender, EventArgs e)
    {
        if (TotalTime - CurrentTime <= _fadeOutThreshold && !_hasFadeOut)
        {
            _hasFadeOut = true;

            _metadata.FadeInOutSampleProvider?.BeginFadeOut(800);
        }
        else if (CurrentTime >= TotalTime)
        {
            await Next();
        }
    }

    public void Dispose()
    {
        StopPlay();

        GC.SuppressFinalize(this);
    }
}