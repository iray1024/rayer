using Microsoft.Extensions.DependencyInjection;
using NAudio.Wave;
using Rayer.Core.Abstractions;
using Rayer.Core.Common;
using Rayer.Core.Events;
using Rayer.Core.Models;
using System.Windows.Threading;

namespace Rayer.Core.Playing;

public class Playback : IDisposable
{
    private WaveMetadata _metadata = new();

    private readonly IPlayQueueProvider _playQueueProvider;
    private readonly IWaveMetadataFactory _metadataFactory;

    private readonly IAudioManager _audioManager = null!;
    private readonly IDeviceManager _deviceManager = null!;

    private static readonly Audio _fallbackAudio = new();

    private static readonly TimeSpan _jumpThreshold = TimeSpan.FromSeconds(5);

    private int _isClickToPlay = 0;

    public Playback(
        IAudioManager audioManager,
        IServiceProvider serviceProvider)
    {
        _audioManager = audioManager;

        _playQueueProvider = serviceProvider.GetRequiredService<IPlayQueueProvider>();
        _metadataFactory = serviceProvider.GetRequiredService<IWaveMetadataFactory>();
        _deviceManager = serviceProvider.GetRequiredService<IDeviceManager>();

        _deviceManager.PlaybackStopped += OnPlaybackStopped;
        _deviceManager.MetadataChanged += OnMetadataChanged;

        Queue.AddRange(_audioManager.Audios);

        DispatcherTimer.Tick += OnTick;
    }

    public IDeviceManager Device => _deviceManager;

    public SortableObservableCollection<Audio> Queue => _playQueueProvider.Queue;

    public bool Repeat { get; set; } = true;

    public bool Shuffle { get; set; } = false;

    public bool Playing { get; set; } = false;

    public bool IsClickToPlay
    {
        get => _isClickToPlay != 0;
        set
        {
            _ = Interlocked.Exchange(ref _isClickToPlay, value ? 1 : 0);
        }
    }

    public TimeSpan CurrentTime
    {
        get => _metadata.Reader is not null ? _metadata.Reader.CurrentTime : TimeSpan.Zero;
        set
        {
            if (_metadata.Reader is not null)
            {
                _metadata.Reader.CurrentTime = value;
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

    public Audio Audio { get; set; } = null!;

    public DispatcherTimer DispatcherTimer { get; } =
        new DispatcherTimer(DispatcherPriority.Render) { Interval = TimeSpan.FromMilliseconds(100) };

    public event EventHandler<AudioPlayingArgs>? AudioPlaying;
    public event EventHandler? AudioPaused;
    public event EventHandler<AudioChangedArgs>? AudioChanged;
    public event EventHandler? AudioStopped;

    public event EventHandler? Seeked;

    public void Initialize(float volume, float pitch, PlayloopMode playloopMode)
    {
        _deviceManager.Volume = volume;
        _deviceManager.Pitch = pitch;

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
            CurrentTime = _metadata.Reader.TotalTime * value * 0.01;

            _metadata.FadeInOutSampleProvider?.BeginFadeIn(1000);

            Seeked?.Invoke(this, EventArgs.Empty);
        }
    }

    public void Jump(bool negative = false)
    {
        if (_metadata.Reader is not null)
        {
            var targetTime = CurrentTime + (_jumpThreshold * (negative ? -1 : 1));

            if (targetTime <= TimeSpan.Zero)
            {
                CurrentTime = TimeSpan.Zero;
            }
            else if (targetTime >= TotalTime)
            {
                ForceStopCurrentDevice();
            }
            else
            {
                CurrentTime = targetTime;
            }

            _metadata.FadeInOutSampleProvider?.BeginFadeIn(1000);

            Seeked?.Invoke(this, EventArgs.Empty);
        }
    }

    public async Task PlayRepeat()
    {
        if (IsClickToPlay)
        {
            IsClickToPlay = false;
        }
        else
        {
            await Load();
            Resume();
        }
    }

    public async Task Play(Audio audio, bool isEventTriggered = false)
    {
        if (!isEventTriggered)
        {
            IsClickToPlay = true;

            await InternalPlay(audio);

            IsClickToPlay = false;
        }
        else
        {
            if (IsClickToPlay)
            {
                IsClickToPlay = false;
            }
            else
            {
                await InternalPlay(audio);
            }
        }

        #region InternalCall
        async Task InternalPlay(Audio audio)
        {
            CheckAudio(ref audio);

            if (Audio != audio)
            {
                Audio = audio;
            }

            await Load();
            Resume();
        }
        #endregion
    }

    public async Task Load()
    {
        ForceStopCurrentDevice();

        _metadata = _metadataFactory.Create(Audio.Path);

        await _deviceManager.LoadAsync(_metadata);

        OpenFile();
    }

    public void Resume(bool fadeIn = true)
    {
        if (_deviceManager.Device is not null && _metadata.Reader is not null && _deviceManager.PlaybackState is not PlaybackState.Playing)
        {
            DispatcherTimer.Start();

            var oldState = _deviceManager.PlaybackState;
#if DEBUG
            Console.WriteLine("开始播放\n");
#endif
            _deviceManager.Device.Play();
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

        _deviceManager.Device?.Pause();

        AudioPaused?.Invoke(this, EventArgs.Empty);
    }

    public void Stop()
    {
        DispatcherTimer.Stop();

        _deviceManager.Stop();

        if (_metadata.Reader is not null)
        {
            CloseFile();
        }
    }

    public void EndPlay()
    {
        DispatcherTimer.Stop();

        Playing = false;

        _deviceManager.Stop();

        if (_metadata.Reader is not null)
        {
            CloseFile();
        }

        Audio = _fallbackAudio;

        AudioStopped?.Invoke(this, EventArgs.Empty);
    }

    public async Task Next(bool isEventTriggered = false)
    {
        var index = GetNextAudioIndex();

        await Play(Queue[index], isEventTriggered);
    }

    public async Task Previous(bool isEventTriggered = false)
    {
        var index = Queue.IndexOf(Audio) - 1;

        index = index < 0 ? Queue.Count - 1 : index;

        await Play(Queue[index], isEventTriggered);
    }

    public void UpdateEqualizer()
    {
        _metadata.Equalizer?.Update();
    }

    private void OpenFile()
    {
        try
        {
            if (_metadata.PitchShiftingSampleProvider is not null)
            {
                _metadata.PitchShiftingSampleProvider.Pitch = _deviceManager.Pitch;
            }

            _deviceManager.Init();

            AudioChanged?.Invoke(this, new AudioChangedArgs() { New = Audio });
        }
        catch (Exception ex)
        {
#if DEBUG
            Console.WriteLine($"\n打开文件出现异常：{ex.Message}\n");
#endif
            CloseFile();
        }
    }

    private void CloseFile()
    {
        _metadata.Dispose();
    }

    private async void OnPlaybackStopped(object? sender, StoppedEventArgs e)
    {
#if DEBUG
        await Console.Out.WriteLineAsync("播放结束事件响应");
#endif
        _deviceManager.Stop();

        if (Repeat && Playing)
        {
            await PlayRepeat();
        }
        else
        {
            await Next(true);
        }
    }

    private void OnMetadataChanged(object? sender, MetadataChangedArgs e)
    {
        _metadata = e.New;

        if (_metadata.PitchShiftingSampleProvider is not null)
        {
            _metadata.PitchShiftingSampleProvider.Pitch = _deviceManager.Pitch;
        }

        _deviceManager.Device?.Play();

        _metadata.FadeInOutSampleProvider?.BeginFadeIn(100);
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

    private void OnTick(object? sender, EventArgs e)
    {
        if (CurrentTime > TotalTime)
        {
            ForceStopCurrentDevice();
        }
    }

    private void ForceStopCurrentDevice()
    {
        DispatcherTimer.Stop();

        _deviceManager.Stop();
    }

    public void Dispose()
    {
        EndPlay();

        GC.SuppressFinalize(this);
    }
}