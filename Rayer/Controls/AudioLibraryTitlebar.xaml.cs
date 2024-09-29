using Rayer.Core;
using Rayer.Core.Framework.Injection;
using Rayer.Views.Pages;
using System.Windows;
using System.Windows.Controls;

namespace Rayer.Controls;

[Inject]
public partial class AudioLibraryTitlebar : UserControl
{
    public AudioLibraryTitlebar()
    {
        InitializeComponent();
    }

    private void OnFilterBoxFocusChanged(object sender, RoutedEventArgs e)
    {
        var page = AppCore.GetRequiredService<AudioLibraryPage>();

        page.OnFilterBoxFocusChanged(this, e);
    }

    private void OnFilterTextChanged(object sender, TextChangedEventArgs e)
    {
        var page = AppCore.GetRequiredService<AudioLibraryPage>();

        page.OnFilterTextChanged(this, e);
    }
}