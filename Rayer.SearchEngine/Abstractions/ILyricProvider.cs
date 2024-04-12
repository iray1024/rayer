using Rayer.Core.Lyric.Data;

namespace Rayer.SearchEngine.Abstractions;

public interface ILyricProvider
{
    LyricData? LyricData { get; }

    event EventHandler<Core.Events.AudioPlayingArgs> AudioPlaying;
    event EventHandler AudioChanged;
    event EventHandler AudioPaused;
    event EventHandler AudioStopped;
}