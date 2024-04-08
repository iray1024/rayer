using Microsoft.Extensions.DependencyInjection;
using NAudio.Wave;
using Rayer.Core.Abstractions;
using Rayer.Core.Events;
using Rayer.Core.Models;
using System.IO;
using System.Windows;

namespace Rayer.Core.Services;

internal class DeviceManager : IDeviceManager
{
    private readonly IServiceProvider _serviceProvider;

    private WaveOutEvent? _device;
    private readonly SemaphoreSlim _semaphore = new(2, 2);

    private WaveMetadata? _metadata;

    private float _volume = 1f;
    private float _pitch = 1f;

    private int _isReOpen = 0;

    public DeviceManager(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    public WaveOutEvent? Device => _device;

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

    public PlaybackState PlaybackState => _device is not null
        ? _device.PlaybackState
        : PlaybackState.Stopped;

    public bool IsReOpen
    {
        get => _isReOpen != 0;
        set
        {
            _ = Interlocked.Exchange(ref _isReOpen, value ? 1 : 0);
        }
    }

    public event EventHandler<StoppedEventArgs>? PlaybackStopped;
    public event EventHandler<MetadataChangedArgs>? MetadataChanged;

    public async Task LoadAsync(WaveMetadata metadata)
    {
        await _semaphore.WaitAsync();
        Stop();

        await EnsureDeviceCreatedAsync(metadata);
    }

    public void Init()
    {
        if (_metadata is null)
        {
            throw new InvalidOperationException("请先加载WaveMetadata");
        }

        _device?.Init(_metadata);
    }

    public void Stop()
    {
        _device?.Stop();

        _device?.Dispose();
        _device = null;
    }

    public async Task SwitchPitchProvider()
    {
        if (_metadata?.Reader is not null)
        {
            _device?.Pause();

            var position = _metadata.Reader.Position;

            var filePath = (_metadata.BaseStream as FileStream)?.Name!;

            var factory = _serviceProvider.GetRequiredService<IWaveMetadataFactory>();

            var metadata = factory.Create(filePath);

            _metadata = metadata;

            if (_metadata?.Reader is not null)
            {
                IsReOpen = true;

                await LoadAsync(metadata);
                Init();

                _metadata.Reader.Position = position;

                MetadataChanged?.Invoke(this, new(_metadata));
            }
        }
    }

    private async Task EnsureDeviceCreatedAsync(WaveMetadata metadata, CancellationToken cancellationToken = default)
    {
        if (_device is null)
        {
            _metadata = metadata;
#if DEBUG
            await Console.Out.WriteLineAsync($"等待设备创建，正在等待同步锁，线程ID: {Environment.CurrentManagedThreadId}");
#endif

            await _semaphore.WaitAsync(cancellationToken);
#if DEBUG
            await Console.Out.WriteLineAsync($"等待设备创建，已获取同步锁，线程ID: {Environment.CurrentManagedThreadId}");
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

        _device = new WaveOutEvent() { Volume = _volume, DesiredLatency = 200 };

        _device.PlaybackStopped += async (s, a) =>
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
        if (_metadata?.PitchShiftingSampleProvider is not null)
        {
            _metadata.PitchShiftingSampleProvider.Pitch = MathF.Round(Pitch, 2, MidpointRounding.ToZero);
        }
    }
}