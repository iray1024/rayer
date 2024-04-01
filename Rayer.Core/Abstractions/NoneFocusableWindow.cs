using Rayer.Core.Utils;
using System.Windows;
using Wpf.Ui.Controls;

namespace Rayer.Core.Abstractions;

public class NoneFocusableWindow : FluentWindow
{
    public NoneFocusableWindow()
    {
        Loaded += OnLoaded;
    }

    private void OnLoaded(object sender, RoutedEventArgs e)
    {
        ElementHelper.TraverseControlsAndSetNoneFocusable(this);
    }
}