using System.Windows.Input;

namespace Rayer.Core.Menu;

public interface ICommandBinding
{
    ICommand PlayCommand { get; }

    ICommand AddToCommand { get; }

    ICommand MoveToCommand { get; }

    ICommand DeleteCommand { get; }
}