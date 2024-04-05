using System.Windows.Input;

namespace Rayer.Core.Abstractions;

public interface ICommandBinding
{
    ICommand PlayCommand { get; }

    ICommand AddToCommand { get; }

    ICommand MoveToCommand { get; }

    ICommand DeleteCommand { get; }
}