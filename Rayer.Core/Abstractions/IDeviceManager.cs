using NAudio.Wave;
using Rayer.Core.Models;

namespace Rayer.Core.Abstractions;

public interface IDeviceManager
{
    WaveOutEvent? Device { get; }

    float Volume { get; set; }

    float Pitch { get; set; }

    PlaybackState PlaybackState { get; }

    Task LoadAsync(WaveMetadata metadata);

    void Init();

    void Stop();

    event EventHandler<StoppedEventArgs> PlaybackStopped;
}