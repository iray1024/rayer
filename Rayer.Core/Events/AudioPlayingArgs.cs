using NAudio.Wave;

namespace Rayer.Core.Events;

public class AudioPlayingArgs : EventArgs
{
    public PlaybackState PlaybackState { get; set; }
}

public delegate void AudioPlayingEventHandler(object? sender, AudioPlayingArgs e);