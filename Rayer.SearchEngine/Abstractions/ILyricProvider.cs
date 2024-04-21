using Rayer.Core.Common;
using Rayer.Core.Lyric.Models;
using Rayer.SearchEngine.Events;

namespace Rayer.SearchEngine.Abstractions;

public interface ILyricProvider
{
    LyricData? LyricData { get; }

    LyricSearcher LyricSearcher { get; }

    Task SwitchSearcherAsync();

    event EventHandler<Rayer.Core.Events.AudioPlayingArgs> AudioPlaying;
    event EventHandler<Rayer.Core.Events.AudioChangedArgs> AudioChanged;
    event EventHandler AudioPaused;
    event EventHandler AudioStopped;
    event EventHandler<SwitchLyricSearcherArgs> LyricChanged;
}