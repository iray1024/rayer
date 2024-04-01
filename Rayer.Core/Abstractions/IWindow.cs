using System.Windows;

namespace Rayer.Core.Abstractions;

public interface IWindow
{
    event RoutedEventHandler Loaded;

    void Show();
}