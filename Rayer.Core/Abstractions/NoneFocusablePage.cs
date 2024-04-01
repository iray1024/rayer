using Rayer.Core.Utils;
using System.Windows;
using System.Windows.Controls;

namespace Rayer.Core.Abstractions;

public class NoneFocusablePage : Page
{
    public NoneFocusablePage()
    {
        Loaded += OnLoaded;
    }

    private void OnLoaded(object sender, RoutedEventArgs e)
    {
        ElementHelper.TraverseControlsAndSetNoneFocusable(this);
    }
}