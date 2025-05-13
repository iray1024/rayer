using Rayer.Core.Common;
using Rayer.Core.Events;
using Rayer.Core.Lyric.Models;

namespace Rayer.Core.Abstractions;

public interface ILyricProvider
{
    LyricData? LyricData { get; }

    LyricSearcher LyricSearcher { get; }

    Task SwitchSearcherAsync();

    void FastForward();

    void FastBackward();

    event EventHandler<AudioPlayingArgs> AudioPlaying;
    event EventHandler<AudioChangedArgs> AudioChanged;
    event EventHandler AudioPaused;
    event EventHandler AudioStopped;
    event EventHandler<SwitchLyricSearcherArgs> LyricChanged;
}