using CommunityToolkit.Mvvm.Input;
using Rayer.Core.Common;
using Rayer.Core.Models;

namespace Rayer.Core.Menu;

public interface ICommandBinding
{
    IAsyncRelayCommand PreviousCommand { get; }
    IRelayCommand PlayOrPauseCommand { get; }
    IAsyncRelayCommand NextCommand { get; }

    IAsyncRelayCommand AddPlaylistCommand { get; }

    IAsyncRelayCommand<string> EditPlaylistCommand { get; }

    IAsyncRelayCommand<string> DeletePlaylistCommand { get; }

    IAsyncRelayCommand<object?> PlayCommand { get; }

    IRelayCommand<PlaylistUpdate> AddToCommand { get; }

    IRelayCommand<PlaylistUpdate> MoveToCommand { get; }

    IRelayCommand<PlaylistUpdate> DeleteFromCommand { get; }

    IAsyncRelayCommand<object?> DeleteCommand { get; }

    IAsyncRelayCommand<LyricSearcher> SwitchLyricSearcherCommand { get; }
    IRelayCommand FastForwardCommand { get; }
    IRelayCommand FastBackwardCommand { get; }
}