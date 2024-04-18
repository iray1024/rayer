using Rayer.Core;
using Rayer.SearchEngine.ViewModels.Explore.LibraryDetail;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Interop;

namespace Rayer.SearchEngine.Controls.Explore.LibraryDetail;

public partial class ExploreLibraryDetailPlaylistPanel : UserControl
{
    public ExploreLibraryDetailPlaylistPanel()
    {
        var vm = AppCore.GetRequiredService<ExploreLibraryDetailPlaylistViewModel>();

        ViewModel = vm;
        DataContext = this;

        InitializeComponent();
    }

    public ExploreLibraryDetailPlaylistViewModel ViewModel { get; set; }

    private void OnLoaded(object sender, System.Windows.RoutedEventArgs e)
    {
        AppCore.MainWindow.SizeChanged += OnSizeChanged;

        var currentWindowWidth = AppCore.MainWindow.ActualWidth;

        var currentScreen = System.Windows.Forms.Screen.FromHandle(new WindowInteropHelper(Application.Current.MainWindow).Handle);

        var factor = (currentWindowWidth + 500) / currentScreen.Bounds.Width;

        var panelWidth = ((currentWindowWidth - 180) / 5) - (100 * Math.Min(factor, 1));

        ViewModel.CoverMaxWidth = panelWidth + 60;
        ViewModel.CoverClip = new System.Windows.Rect(0, 0, ViewModel.CoverMaxWidth, ViewModel.CoverMaxWidth);
    }

    private void OnUnloaded(object sender, System.Windows.RoutedEventArgs e)
    {
        AppCore.MainWindow.SizeChanged -= OnSizeChanged;
    }

    private void OnSizeChanged(object sender, System.Windows.SizeChangedEventArgs e)
    {
        var currentScreen = System.Windows.Forms.Screen.FromHandle(new WindowInteropHelper(Application.Current.MainWindow).Handle);

        var factor = (e.NewSize.Width + 500) / currentScreen.Bounds.Width;

        var panelWidth = ((e.NewSize.Width - 180) / 5) - (100 * Math.Min(factor, 1));

        ViewModel.CoverMaxWidth = panelWidth + 60;
        ViewModel.CoverClip = new System.Windows.Rect(0, 0, ViewModel.CoverMaxWidth, ViewModel.CoverMaxWidth);
    }
}