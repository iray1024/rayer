using CommunityToolkit.Mvvm.ComponentModel;
using Rayer.SearchEngine.Views.Pages;
using Rayer.Views.Pages;
using System.Collections.ObjectModel;
using Wpf.Ui.Controls;

namespace Rayer.ViewModels;

public partial class MainWindowViewModel : ObservableObject
{
    [ObservableProperty]
    private string _applicationTitle = "Rayer-Music";

    [ObservableProperty]
    private ICollection<object> _menuItems =
    [
         new NavigationViewItem("乐库", SymbolRegular.Home24, typeof(AudioLibraryPage)),
        new NavigationViewItem("探索", SymbolRegular.Home24, typeof(ExplorePage)),
    ];

    [ObservableProperty]
    private ICollection<object> _footerMenuItems =
    [
        new NavigationViewItem("设置", SymbolRegular.Settings24, typeof(SettingsPage))
    ];

    [ObservableProperty]
    private ObservableCollection<MenuItem> _trayMenuItems =
    [
        new MenuItem { Header = "乐库", Tag = "tray_audioLibrary" },
        new MenuItem { Header = "退出", Tag = "tray_exit" }
    ];
}