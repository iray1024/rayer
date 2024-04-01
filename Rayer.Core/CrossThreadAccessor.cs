using System.Windows;

namespace Rayer.Core;

public class CrossThreadAccessor
{
    private static Action<Action, bool> _executor =
           (action, async) => action();

    internal static void Initialize()
    {
        var dispatcher = Application.Current.Dispatcher;

        _executor = (action, async) =>
        {
            if (dispatcher.CheckAccess())
            {
                action();
            }
            else
            {
                if (async)
                {
                    dispatcher.BeginInvoke(action);
                }
                else
                {
                    dispatcher.Invoke(action);
                }
            }
        };
    }

    public static void Run(Action action)
    {
        _executor(action, false);
    }

    public static void RunAsync(Action action)
    {
        _executor(action, true);
    }
}
