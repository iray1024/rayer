using Rayer.Core;
using Rayer.Core.Utils;
using Rayer.Services;
using Rayer.ViewModels;
using System.Windows.Media;
using Wpf.Ui;
using Wpf.Ui.Abstractions.Controls;
using Wpf.Ui.Appearance;
using Wpf.Ui.Controls;

namespace Rayer.Views.Pages;

public partial class SettingsPage : INavigableView<SettingsViewModel>
{
    public SettingsPage(SettingsViewModel viewModel)
    {
        ViewModel = viewModel;
        DataContext = this;

        InitializeComponent();

        ApplicationThemeManager.Changed += OnThemeChanged;
    }

    public SettingsViewModel ViewModel { get; }

    private void OnThemeChanged(ApplicationTheme currentApplicationTheme, Color systemAccent)
    {
        PitchProviderSetting.Icon = new ImageIcon
        {
            Source = (ImageSource)StaticThemeResources.GetDynamicResource("Pitch"),
            Width = 24,
            Height = 24
        };

        LyricSearcherSetting.Icon = new ImageIcon
        {
            Source = (ImageSource)StaticThemeResources.GetDynamicResource("Lyric"),
            Width = 24,
            Height = 24
        };

        SearcherSetting.Icon = new ImageIcon
        {
            Source = (ImageSource)StaticThemeResources.GetDynamicResource("Search"),
            Width = 24,
            Height = 24
        };
    }

    private void OnLoaded(object sender, System.Windows.RoutedEventArgs e)
    {
        var navView = AppCore.GetRequiredService<INavigationService>().GetNavigationControl() as NavigationView;

        if (navView?.Template.FindName("PART_NavigationViewContentPresenter", navView) is NavigationViewContentPresenter navPresenter)
        {
            var scrollViewer = ElementHelper.GetScrollViewer(navPresenter);

            scrollViewer?.ScrollToTop();
        }
    }
}