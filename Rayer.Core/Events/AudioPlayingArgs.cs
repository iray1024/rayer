using NAudio.Wave;

namespace Rayer.Core.Events;

public class AudioPlayingArgs : EventArgs
{
    public PlaybackState PlaybackState { get; set; }
}