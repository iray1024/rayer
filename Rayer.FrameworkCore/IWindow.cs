using System.Windows;

namespace Rayer.FrameworkCore;

public interface IWindow
{
    event RoutedEventHandler Loaded;

    void Show();
}