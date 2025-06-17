using Microsoft.Extensions.DependencyInjection;
using NAudio.Wave;
using Rayer.Core.Abstractions;
using Rayer.Core.Events;
using Rayer.Core.Models;
using Rayer.FrameworkCore;
using Rayer.FrameworkCore.Injection;
using System.Diagnostics;
using System.Windows;

namespace Rayer.Core.Services;

[Inject<IDeviceManager>]
internal class DeviceManager(IServiceProvider serviceProvider) : IDeviceManager
{
    private readonly SemaphoreSlim _semaphore = new(2, 2);

    private WaveMetadata? _metadata;

    private float _volume = 1f;
    private float _pitch = 1f;
    private float _speed = 1f;

    private int _isReOpen = 0;

    public WaveOutEvent? Device { get; private set; }

    public float Volume
    {
        get => _volume;
        set
        {
            _volume = Math.Min(Math.Max(value, 0), 1);
            SetVolume();
        }
    }

    public float Pitch
    {
        get => _pitch;
        set
        {
            _pitch = Math.Min(Math.Max(value, 0.5f), 2);
            SetPitch();
        }
    }

    public float Speed
    {
        get => _speed;
        set
        {
            _speed = Math.Min(Math.Max(value, 0f), 2f);
            SetSpeed();
        }
    }

    public PlaybackState PlaybackState => Device is not null
        ? Device.PlaybackState
        : PlaybackState.Stopped;

    public bool IsReOpen
    {
        get => _isReOpen != 0;
        set => _ = Interlocked.Exchange(ref _isReOpen, value ? 1 : 0);
    }

    public event EventHandler<StoppedEventArgs>? PlaybackStopped;
    public event EventHandler<MetadataChangedArgs>? MetadataChanged;

    public async Task LoadAsync(WaveMetadata metadata)
    {
        await _semaphore.WaitAsync().ConfigureAwait(false);
        Stop();

        await EnsureDeviceCreatedAsync(metadata);
    }

    public void Init()
    {
        if (_metadata is null)
        {
            throw new InvalidOperationException("请先加载WaveMetadata");
        }

        if (_metadata.PitchShiftingSampleProvider is not null)
        {
            _metadata.PitchShiftingSampleProvider.Pitch = Pitch;
        }

        SetSpeed();

        Device?.Init(_metadata);
    }

    public void Stop()
    {
        Device?.Stop();

        Device?.Dispose();
        Device = null;
    }

    public async Task SwitchPitchProvider()
    {
        if (_metadata?.Reader is not null)
        {
            Device?.Pause();

            var currentTime = _metadata.Reader.CurrentTime;

            var factory = serviceProvider.GetRequiredService<IWaveMetadataFactory>();

            var metadata = await factory.CreateAsync(_metadata.Uri);

            if (metadata is not null)
            {
                _metadata = metadata;

                if (_metadata?.Reader is not null)
                {
                    IsReOpen = true;

                    await LoadAsync(metadata);
                    Init();

                    _metadata.Reader.CurrentTime = currentTime;

                    MetadataChanged?.Invoke(this, new(_metadata));
                }
            }
        }
    }

    private async Task EnsureDeviceCreatedAsync(WaveMetadata metadata, CancellationToken cancellationToken = default)
    {
        if (Device is null)
        {
            _metadata = metadata;
#if DEBUG
            Debug.WriteLine($"等待设备创建，正在等待同步锁");
#endif

            await _semaphore.WaitAsync(cancellationToken).ConfigureAwait(false);
#if DEBUG
            Debug.WriteLine($"等待设备创建，已获取同步锁");
#endif
            CreateDevice();
            _semaphore.Release();
        }
    }

    private void CreateDevice()
    {
        if (_metadata is null)
        {
            throw new InvalidOperationException("请先加载WaveMetadata");
        }

        Device = new WaveOutEvent() { Volume = _volume, DesiredLatency = 200 };

        Device.PlaybackStopped += async (s, a) =>
        {
            try
            {
                await Task.Run(async () =>
                {
                    await Application.Current.Dispatcher.InvokeAsync(() =>
                    {
                        _semaphore.Release();

                        if (!IsReOpen)
                        {
                            PlaybackStopped?.Invoke(s, a);
                        }
                        else
                        {
                            IsReOpen = false;
                        }
                    });
                });
            }
            catch (TaskCanceledException) when (AppCore.MainWindow is not null && AppCore.MainWindow.IsActive)
            {
                throw;
            }
        };
    }

    private void SetVolume()
    {
        if (Device is not null)
        {
            Device.Volume = _volume;
        }
    }

    private void SetPitch()
    {
        if (_metadata?.PitchShiftingSampleProvider is not null)
        {
            _metadata.PitchShiftingSampleProvider.Pitch = MathF.Round(Pitch, 2, MidpointRounding.ToZero);
        }
    }

    private void SetSpeed()
    {
#pragma warning disable IDE0045
        if (_metadata?.TempoChangeProvider is not null)
        {
            if (Speed == 2)
            {
                _metadata.TempoChangeProvider.TempoChange = 100;
            }
            else if (Speed > 1)
            {
                _metadata.TempoChangeProvider.TempoChange = (Speed * 100) - 100;
            }
            else if (Speed == 1)
            {
                _metadata.TempoChangeProvider.TempoChange = 0;
            }
            else
            {
                _metadata.TempoChangeProvider.TempoChange = -50 + (Speed * 50);
            }
        }
#pragma warning restore IDE0045
    }
}