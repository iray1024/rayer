using Microsoft.Extensions.DependencyInjection;
using NAudio.Wave;
using Rayer.Core.Abstractions;
using Rayer.Core.Common;
using Rayer.Core.Events;
using Rayer.Core.Models;
using Rayer.Core.PlayControl.Abstractions;
using Rayer.FrameworkCore;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Windows;
using System.Windows.Threading;
using Wpf.Ui;
using Wpf.Ui.Extensions;

namespace Rayer.Core.Playing;

public class Playback : IDisposable
{
    private WaveMetadata _metadata = new();

    private readonly IPlayQueueProvider _playQueueProvider;
    private readonly IWaveMetadataFactory _metadataFactory;

    private readonly IAudioManager _audioManager = null!;
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
        DeviceManager = serviceProvider.GetRequiredService<IDeviceManager>();

        DeviceManager.PlaybackStopped += OnPlaybackStopped;
        DeviceManager.MetadataChanged += OnMetadataChanged;

        Queue.AddRange(_audioManager.Audios);

        DispatcherTimer.Tick += OnTick;
    }

    public IDeviceManager DeviceManager { get; } = null!;

    public SortableObservableCollection<Audio> Queue => _playQueueProvider.Queue;

    public bool Repeat { get; set; } = true;

    public bool Shuffle { get; set; } = false;

    public bool Playing { get; set; } = false;

    public bool IsClickToPlay
    {
        get => _isClickToPlay != 0;
        set => _ = Interlocked.Exchange(ref _isClickToPlay, value ? 1 : 0);
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

    public bool IsSeeking { get; set; } = false;

    public event EventHandler<AudioPlayingArgs>? AudioPlaying;
    public event EventHandler? AudioPaused;
    public event EventHandler<AudioChangedArgs>? AudioChanged;
    public event EventHandler? AudioStopped;

    public event EventHandler? Seeked;

    public void Initialize(float volume, float pitch, PlayloopMode playloopMode)
    {
        DeviceManager.Volume = volume;
        DeviceManager.Pitch = pitch;

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
            CurrentTime = TimeSpan.FromSeconds(_metadata.Reader.TotalTime.TotalSeconds * value * 0.01);

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

        var metadata = await _metadataFactory.CreateAsync(Audio.Path);

        if (metadata is not null)
        {
            _metadata = metadata;

            await DeviceManager.LoadAsync(_metadata);

            OpenFile();
        }
        else
        {
            var dialogService = AppCore.GetRequiredService<IContentDialogService>();

            if (dialogService is not null)
            {
                await Application.Current.Dispatcher.InvokeAsync(async () =>
                {
                    await dialogService.ShowSimpleDialogAsync(new SimpleContentDialogCreateOptions
                    {
                        Title = "出现错误",
                        Content = $"无法播放当前歌曲",
                        CloseButtonText = "关闭"
                    }, AppCore.StoppingToken);
                });
            }
        }
    }

    public void Resume(bool fadeIn = true)
    {
        if (DeviceManager.Device is not null && _metadata.Reader is not null && DeviceManager.PlaybackState is not PlaybackState.Playing)
        {
            DispatcherTimer.Start();

            var oldState = DeviceManager.PlaybackState;
#if DEBUG
            Console.WriteLine("开始播放\n");
#endif
            DeviceManager.Device.Play();
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

        DeviceManager.Device?.Pause();

        AudioPaused?.Invoke(this, EventArgs.Empty);
    }

    public void Stop()
    {
        DispatcherTimer.Stop();

        DeviceManager.Stop();

        if (_metadata.Reader is not null)
        {
            CloseFile();
        }
    }

    public void EndPlay()
    {
        DispatcherTimer.Stop();

        Playing = false;

        DeviceManager.Stop();

        if (_metadata.Reader is not null)
        {
            CloseFile();
        }

        Audio = _fallbackAudio;

        AudioStopped?.Invoke(this, EventArgs.Empty);
    }

    [return: MaybeNull]
    public bool TryGetAudio(string id, out Audio audio)
    {
        audio = null!;

        foreach (var item in Queue)
        {
            if (item.Id == id)
            {
                audio = item;

                return true;
            }
        }

        return false;
    }

    public async Task Next(bool isEventTriggered = false)
    {
        var index = GetNextAudioIndex();

        if (index != -1)
        {
            await Play(Queue[index], isEventTriggered);
        }
        else
        {
            EndPlay();
        }
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
                _metadata.PitchShiftingSampleProvider.Pitch = DeviceManager.Pitch;
            }

            DeviceManager.Init();

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
        if (IsClickToPlay)
        {
            return;
        }

#if DEBUG
        Debug.WriteLine("播放结束事件响应");
#endif
        DeviceManager.Stop();

        if (Repeat && Playing)
        {
            await PlayRepeat();
        }
        else if (Playing)
        {
            await Next(true);
        }
    }

    private void OnMetadataChanged(object? sender, MetadataChangedArgs e)
    {
        _metadata = e.New;

        if (_metadata.PitchShiftingSampleProvider is not null)
        {
            _metadata.PitchShiftingSampleProvider.Pitch = DeviceManager.Pitch;
        }

        DeviceManager.Device?.Play();

        _metadata.FadeInOutSampleProvider?.BeginFadeIn(500);
    }

    private int GetNextAudioIndex()
    {
        var currentIndex = Queue.IndexOf(Audio);

        var index = Shuffle ? GetExcludeRandomIndex(currentIndex) : currentIndex + 1;

        return Queue.Count > 0
            ? index >= Queue.Count
                ? 0
                : index
            : -1;
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

        DeviceManager.Stop();
    }

    private int GetExcludeRandomIndex(int exclude)
    {
        int index;

        index = Random.Shared.Next(0, Queue.Count);

        if (index == exclude)
        {
            index = GetExcludeRandomIndex(exclude);
        }

        return index;
    }

    public void Dispose()
    {
        EndPlay();

        GC.SuppressFinalize(this);
    }
}