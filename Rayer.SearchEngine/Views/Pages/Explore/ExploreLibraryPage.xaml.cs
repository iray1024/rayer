using Rayer.Core;
using Rayer.Core.Framework;
using Rayer.Core.Framework.Injection;
using Rayer.Core.Utils;
using Rayer.SearchEngine.Controls.Explore;
using Rayer.SearchEngine.Core.Business.Data;
using Rayer.SearchEngine.Core.Business.Login;
using Rayer.SearchEngine.ViewModels.Explore;
using System.Windows;
using System.Windows.Data;
using Wpf.Ui;
using Wpf.Ui.Controls;

namespace Rayer.SearchEngine.Views.Pages.Explore;

[Inject]
public partial class ExploreLibraryPage : INavigableView<ExploreLibraryViewModel>
{
    public ExploreLibraryPage()
    {
        var vm = (ExploreLibraryViewModel)AppCore.GetRequiredService<IExploreLibraryDataProvider>();

        ViewModel = vm;
        DataContext = this;

        InitializeComponent();
    }

    public ExploreLibraryViewModel ViewModel { get; set; }

    private async void OnLoaded(object sender, RoutedEventArgs e)
    {
        var navView = AppCore.GetRequiredService<INavigationService>().GetNavigationControl() as NavigationView;

        if (navView?.Template.FindName("PART_NavigationViewContentPresenter", navView) is NavigationViewContentPresenter navPresenter)
        {
            var scrollViewer = ElementHelper.GetScrollViewer(navPresenter);

            scrollViewer?.ScrollToTop();
        }

        ViewModel ??= (ExploreLibraryViewModel)AppCore.GetRequiredService<IExploreLibraryDataProvider>();

        ViewModel.LoginSucceed += OnLoginSucceed;

        var loader = AppCore.GetRequiredService<ILoaderProvider>();

        loader.Loading();

        await Task.Run(() => ViewModel.OnLoadAsync());

        var login = AppCore.GetRequiredService<ILoginManager>();
    }

    private void OnUnloaded(object sender, RoutedEventArgs e)
    {
        ViewModel.LoginSucceed -= OnLoginSucceed;
        ViewModel.Unload();

        ViewModel = default!;

        BindingOperations.ClearAllBindings(this);

        GC.Collect();
    }

    private void OnLoginSucceed(object? sender, EventArgs e)
    {
        Application.Current.Dispatcher.Invoke(() => ViewPanel.Visibility = Visibility.Visible);
    }

    private void OnMyFavoriteMouseLeftButtonUp(object sender, System.Windows.Input.MouseButtonEventArgs e)
    {
        var nav = AppCore.GetRequiredService<INavigationService>();

        var loader = AppCore.GetRequiredService<ILoaderProvider>();
        loader.Loading();

        nav.Navigate(typeof(ExplorePlaylistPanel), ViewModel.Model.FavoriteList);
    }
}