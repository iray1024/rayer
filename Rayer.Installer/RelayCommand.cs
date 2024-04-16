using System.Diagnostics;
using System.Windows.Input;

namespace Rayer.Installer;

public class RelayCommand(Action<object?> execute, Predicate<object?>? canExecute) : ICommand
{
    private readonly Action<object?> _execute = execute ?? throw new ArgumentNullException(nameof(execute));

    public RelayCommand(Action<object?> execute)
        : this(execute, null)
    {

    }

    [DebuggerStepThrough]
    public bool CanExecute(object? parameters)
    {
        return canExecute == null || canExecute(parameters);
    }

    public event EventHandler? CanExecuteChanged
    {
        add { CommandManager.RequerySuggested += value; }
        remove { CommandManager.RequerySuggested -= value; }
    }

    public static void RaiseCanExecuteChanged()
    {
        CommandManager.InvalidateRequerySuggested();
    }

    public void Execute(object? parameters)
    {
        _execute(parameters);
    }
}