using CommunityToolkit.Mvvm.Input;
using Rayer.Core.Common;

namespace Rayer.Core.Menu;

public interface ICommandBinding
{
    IAsyncRelayCommand PreviousCommand { get; }
    IRelayCommand PlayOrPauseCommand { get; }
    IAsyncRelayCommand NextCommand { get; }

    IAsyncRelayCommand AddPlaylistCommand { get; }

    IAsyncRelayCommand<object?> PlayCommand { get; }

    IRelayCommand<object?> AddToCommand { get; }

    IRelayCommand<object?> MoveToCommand { get; }

    IAsyncRelayCommand<object?> DeleteCommand { get; }

    IAsyncRelayCommand<LyricSearcher> SwitchLyricSearcherCommand { get; }
    IRelayCommand FastForwardCommand { get; }
    IRelayCommand FastBackwardCommand { get; }
}