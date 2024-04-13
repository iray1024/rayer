using Rayer.Core.Framework;
using Rayer.Core.Framework.Injection;
using Rayer.SearchEngine.Views.Pages;
using Rayer.SearchEngine.Views.Pages.Explore;
using System.Windows;
using System.Windows.Media;
using Wpf.Ui.Controls;

namespace Rayer.SearchEngine.Navigation;

[Inject<INavigationMenuPlugin>]
internal class NavigationMenuPlugin : INavigationMenuPlugin
{
    public ICollection<object> MenuItems { get; } =
    [
        new NavigationViewItem("云音乐", typeof(ExploreHomePage))
        {
            Icon = new ImageIcon()
            {
                Source = (ImageSource)Application.Current.Resources["CloudMusic"],
                Width = 24,
                Height = 24,
            },
            TargetPageTag = "CloudMusic"
        },
        new NavigationViewItem("发现", typeof(ExploreSpotPage))
        {
            Icon = new ImageIcon()
            {
                Source = (ImageSource)Application.Current.Resources["Spot"],
                Width = 24,
                Height = 24,
            },
            TargetPageTag = "Spot"
        },
        new NavigationViewItem("我的", SymbolRegular.Home24, typeof(ExploreLibraryPage))
        {
            Icon = new ImageIcon()
            {
                Source = (ImageSource)Application.Current.Resources["User"],
                Width = 24,
                Height = 24,
            },
            TargetPageTag = "User"
        },
        new NavigationViewItem("搜索", SymbolRegular.Home24, typeof(SearchPage))
        {
            Icon = new ImageIcon()
            {
                Source = (ImageSource)Application.Current.Resources["User"],
                Width = 24,
                Height = 24,
            },
            TargetPageTag = "Search"
        },
    ];
}