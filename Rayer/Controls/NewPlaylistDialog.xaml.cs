using System.Windows.Controls;
using Wpf.Ui.Controls;

namespace Rayer.Controls;

public partial class NewPlaylistDialog : ContentDialog
{
    public NewPlaylistDialog(ContentPresenter? contentPresenter)
        : base(contentPresenter)
    {
        InitializeComponent();

        Loaded += OnLoaded;
        Unloaded += OnUnloaded;
    }

    private void OnLoaded(object sender, System.Windows.RoutedEventArgs e)
    {
        PlaylistName.Focus();
    }

    private void OnUnloaded(object sender, System.Windows.RoutedEventArgs e)
    {
        Loaded -= OnLoaded;
        Unloaded -= OnUnloaded;
    }
}