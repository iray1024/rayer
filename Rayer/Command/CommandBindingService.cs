using Rayer.Command.Parameter;
using Rayer.Core.Abstractions;
using Rayer.Core.Common;
using Rayer.Core.Framework.Injection;
using Rayer.Core.Menu;
using Rayer.Core.PlayControl.Abstractions;
using System.Windows.Input;

namespace Rayer.Command;

[Inject<ICommandBinding>]
internal class CommandBindingService : ICommandBinding
{
    private readonly IAudioManager _audioManager;
    private readonly IPlaylistService _playlistService;

    public CommandBindingService(IAudioManager audioManager, IPlaylistService playlistService)
    {
        _audioManager = audioManager;
        _playlistService = playlistService;
    }

    private ICommand _playCommand = default!;
    public ICommand PlayCommand
    {
        get
        {
            _playCommand ??= new RelayCommand(Play);

            return _playCommand;
        }
    }

    private ICommand _addToCommand = default!;
    public ICommand AddToCommand
    {
        get
        {
            _addToCommand ??= new RelayCommand(AddTo);

            return _addToCommand;
        }
    }

    private ICommand _moveToCommand = default!;
    public ICommand MoveToCommand
    {
        get
        {
            _moveToCommand ??= new RelayCommand(MoveTo);

            return _moveToCommand;
        }
    }

    private ICommand _deleteCommand = default!;

    public ICommand DeleteCommand
    {
        get
        {
            _deleteCommand ??= new RelayCommand(Delete);

            return _deleteCommand;
        }
    }

    private async void Play(object? sender)
    {
        if (sender is AudioCommandParameter parameter)
        {
            await _audioManager.Playback.Play(parameter.Audio);
        }
    }

    private void AddTo(object? sender)
    {

    }

    private void MoveTo(object? sender)
    {

    }

    private async void Delete(object? sender)
    {
        if (sender is AudioCommandParameter parameter)
        {
            if (parameter.Scope is ContextMenuScope.PlayQueue)
            {
                if (_audioManager.Playback.Playing &&
                    _audioManager.Playback.Audio.Equals(parameter.Audio))
                {
                    if (_audioManager.Playback.Queue.Count > 1)
                    {
                        await _audioManager.Playback.Next();
                    }
                    else
                    {
                        _audioManager.Playback.EndPlay();
                    }
                }

                _audioManager.Playback.Queue.Remove(parameter.Audio);
            }
        }
    }
}