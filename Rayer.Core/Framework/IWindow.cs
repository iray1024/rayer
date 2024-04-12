using System.Windows;

namespace Rayer.Core.Framework;

public interface IWindow
{
    event RoutedEventHandler Loaded;

    void Show();
}