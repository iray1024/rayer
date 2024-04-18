using Rayer.Core;
using Rayer.Core.Framework.Injection;
using Rayer.SearchEngine.Abstractions;
using Rayer.SearchEngine.Business.Data.Abstractions;
using Rayer.SearchEngine.Business.Login.Abstractions;
using Rayer.SearchEngine.ViewModels.Explore;
using System.Windows;
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
        ViewModel.LoginSucceed += OnLoginSucceed;

        var loader = AppCore.GetRequiredService<ILoaderProvider>();

        loader.Loading();

        await ViewModel.OnLoadAsync();

        var login = AppCore.GetRequiredService<ILoginManager>();
    }

    private void OnUnloaded(object sender, RoutedEventArgs e)
    {
        ViewModel.LoginSucceed -= OnLoginSucceed;
        ViewModel.Unload();
    }

    private void OnLoginSucceed(object? sender, EventArgs e)
    {
        ViewPanel.Visibility = Visibility.Visible;
    }
}