using NAudio.Wave;
using Rayer.Core.Events;
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

    Task SwitchPitchProvider();

    event EventHandler<StoppedEventArgs> PlaybackStopped;
    event EventHandler<MetadataChangedArgs> MetadataChanged;
}