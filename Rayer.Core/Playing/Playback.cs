using NAudio.Wave;
using NAudio.Wave.SampleProviders;
using Rayer.Core.Abstractions;
using Rayer.Core.Models;
using System.Windows.Threading;

namespace Rayer.Core.Playing;

public class Playback : IDisposable
{
    private WaveOutEvent? _device;
    private AudioFileReader? _reader;
    private FadeInOutSampleProvider? _fadeInOut;

    private readonly IAudioManager _audioManager = null!;

    private readonly SemaphoreSlim _semaphore = new(2, 2);

    private static readonly TimeSpan _fadeOutThreshold = TimeSpan.FromMilliseconds(1000);

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

    public bool Repeat { get; set; } = true;

    public bool Random { get; set; } = false;

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

    private void OnTick(object? sender, EventArgs e)
    {
        if (TotalTime - CurrentTime <= _fadeOutThreshold && !_hasFadeOut)
        {
            _hasFadeOut = true;

            _fadeInOut?.BeginFadeOut(500);
        }
    }

    public IList<Audio> Queue { get; } = [];

    public bool Playing { get; set; } = false;

    public void Seek(double value)
    {
        if (_reader is not null)
        {
            if (_hasFadeOut)
            {
                _hasFadeOut = false;
                _fadeInOut?.BeginFadeIn(100);
            }
            _reader.Position = (long)(_reader.Length * value * 0.01);
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

            _device.Play();
            if (fadeIn)
            {
                _fadeInOut?.BeginFadeIn(1000);
            }

            Playing = true;
        }
    }

    public void Pause()
    {
        DispatcherTimer.Stop();

        _device?.Pause();
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

        _audioManager.Stop();
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
            var inputStream = new AudioFileReader(Audio.Path);
            _reader = inputStream;

            var pitch = new SmbPitchShiftingSampleProvider(_reader.ToSampleProvider())
            {
                //PitchFactor = (float)0.9f
            };

            var fadeInOut = new FadeInOutSampleProvider(pitch);

            _fadeInOut = fadeInOut;

            _device?.Init(fadeInOut);

            _audioManager.Switch(Audio);
        }
        catch (Exception ex)
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
    }

    private void CreateDevice()
    {
        _device = new WaveOutEvent() { Volume = _mute ? 0 : _volume, DesiredLatency = 200 };

        _device.PlaybackStopped += async (s, a) =>
        {
            if (_reader is not null)
            {
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

    private int GetNextAudioIndex()
    {
        var index = Random
            ? System.Random.Shared.Next(0, Queue.Count)
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

    public void Dispose()
    {
        Stop();

        GC.SuppressFinalize(this);
    }
}