using NAudio.Wave;
using NAudio.Wave.SampleProviders;
using Rayer.Core.Abstractions;
using Rayer.Core.Common;
using Rayer.Core.Models;
using System.Collections.ObjectModel;
using System.IO;
using System.Windows;
using System.Windows.Threading;

namespace Rayer.Core.Playing;

public class Playback : IDisposable
{
    private WaveOutEvent? _device;
    private WaveStream? _reader;
    private Stream? _baseStream;
    private FadeInOutSampleProvider? _fadeInOut;
    private SmbPitchShiftingSampleProvider? _pitchProvider;

    private readonly MediaFoundationReader.MediaFoundationReaderSettings _meidaSettings = new() { RequestFloatOutput = true };

    private readonly IAudioManager _audioManager = null!;

    private readonly SemaphoreSlim _semaphore = new(2, 2);

    private static readonly Audio _fallbackAudio = new();

    private static readonly TimeSpan _jumpThreshold = TimeSpan.FromSeconds(5);
    private static readonly TimeSpan _fadeOutThreshold = TimeSpan.FromMilliseconds(1000);

    private static readonly float _semitone = MathF.Pow(2, 1.0f / 12);
    private static readonly float _upOneTone = _semitone * _semitone;
    private static readonly float _downOneTone = 1.0f / _upOneTone;

    private bool _hasFadeOut = false;

    private bool _mute = false;
    public bool Mute
    {
        get => _mute;
        set
        {
            _mute = value;
            SetMute();
        }
    }

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
        get => _reader != null ? _reader.CurrentTime : TimeSpan.Zero;
        set
        {
            if (_reader != null)
            {
                _reader.CurrentTime = value;
            }
        }
    }

    public TimeSpan TotalTime => _reader != null ? _reader.TotalTime : TimeSpan.Zero;

    public long Position
    {
        get => _reader is not null ? _reader.Position : 0;
        set
        {
            if (_reader is not null)
            {
                _reader.Position = value;
            }
        }
    }

    public long Length => _reader is not null ? _reader.Length : 0;

    public PlaybackState PlaybackState => _device is not null
        ? _device.PlaybackState
        : PlaybackState.Stopped;

    public Audio Audio { get; set; } = null!;

    public DispatcherTimer DispatcherTimer { get; } =
        new DispatcherTimer(DispatcherPriority.Render) { Interval = TimeSpan.FromMilliseconds(100) };

    public Playback(Playlist playlist)
    {
        Queue = [.. playlist.Audios];
    }

    public Playback(Audio audio)
    {
        Audio = audio;
    }

    public Playback(IAudioManager audioManager)
    {
        _audioManager = audioManager;

        DispatcherTimer.Tick += OnTick;
    }

    public ObservableCollection<Audio> Queue { get; } = [];

    public bool Playing { get; set; } = false;

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
        if (_reader is not null)
        {
            _reader.Position = (long)(_reader.Length * value * 0.01);

            if (_hasFadeOut)
            {
                _hasFadeOut = false;
                _fadeInOut?.BeginFadeIn(100);
            }
        }
    }

    public void Jump(bool negative = false)
    {
        if (_reader is not null)
        {
            var threshold = (long)(_reader.Length * (_jumpThreshold / _reader.TotalTime));

            var targetPosition = _reader.Position + (threshold * (negative ? -1 : 1));

            _reader.Position = targetPosition < 0
                ? 0
                : targetPosition > _reader.Length
                    ? _reader.Length - 10000
                    : targetPosition;

            if (_hasFadeOut)
            {
                _hasFadeOut = false;
                _fadeInOut?.BeginFadeIn(100);
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
        if (_device is not null && _reader is not null && _device.PlaybackState is not PlaybackState.Playing)
        {
            DispatcherTimer.Start();

            var oldState = _device.PlaybackState;

            _device.Play();
            if (fadeIn)
            {
                _fadeInOut?.BeginFadeIn(1000);
            }

            Playing = true;

            _audioManager.OnPlaying(oldState);
        }
    }

    public void Pause()
    {
        DispatcherTimer.Stop();

        _device?.Pause();

        _audioManager.OnPaused();
    }

    public void Stop()
    {
        DispatcherTimer.Stop();

        Playing = false;

        _device?.Stop();

        if (_reader is not null)
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

        if (_reader is not null)
        {
            CloseFile();
        }

        _device?.Dispose();
        _device = null;

        Audio = _fallbackAudio;

        _audioManager.OnStopped();
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
            _baseStream = new FileStream(Audio.Path, FileMode.Open, FileAccess.Read, FileShare.Read);

            _reader = new StreamMediaFoundationReader(_baseStream, _meidaSettings);

            _pitchProvider = new SmbPitchShiftingSampleProvider(_reader.ToSampleProvider())
            {
                PitchFactor = Pitch
            };

            var fadeInOut = new FadeInOutSampleProvider(_pitchProvider);

            _fadeInOut = fadeInOut;

            _device?.Init(fadeInOut);

            _audioManager.OnSwitch(Audio);
        }
        catch
        {
            CloseFile();
        }
    }

    private void CloseFile()
    {
        _fadeInOut = null;

        _reader?.Close();
        _reader?.Dispose();
        _reader = null;

        _baseStream?.Close();
        _baseStream?.Dispose();

        _baseStream = null;
    }

    private void CreateDevice()
    {
        _device = new WaveOutEvent() { Volume = _mute ? 0 : _volume, DesiredLatency = 200 };

        _device.PlaybackStopped += async (s, a) =>
        {
            await Task.Run(async () =>
            {
                await Application.Current.Dispatcher.InvokeAsync(async () =>
                {
                    if (_reader is not null)
                    {
                        _semaphore.Release(2 - _semaphore.CurrentCount);

                        _reader.Position = 0;

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

    private void SetMute()
    {
        if (_device is not null)
        {
            _device.Volume = _mute ? 0 : _volume;
        }
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
        if (_pitchProvider is not null)
        {
            _pitchProvider.PitchFactor = MathF.Round(Pitch, 2, MidpointRounding.ToZero);
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

    private void OnTick(object? sender, EventArgs e)
    {
        if (TotalTime - CurrentTime <= _fadeOutThreshold && !_hasFadeOut)
        {
            _hasFadeOut = true;

            _fadeInOut?.BeginFadeOut(500);
        }
    }

    public void Dispose()
    {
        StopPlay();

        GC.SuppressFinalize(this);
    }
}