using Rayer.Abstractions;
using Rayer.Core;
using Rayer.Core.Framework;
using Rayer.Core.Framework.Injection;
using Rayer.Core.Framework.Settings.Abstractions;
using Rayer.SearchEngine.Views.Windows;
using Rayer.ViewModels;
using Rayer.Views.Pages;
using System.ComponentModel;
using System.Windows;
using System.Windows.Media;
using Wpf.Ui;
using Wpf.Ui.Appearance;
using Wpf.Ui.Controls;

namespace Rayer;

[Inject<IWindow>]
public partial class MainWindow : IWindow
{
    private bool _isUserClosedPane;
    private bool _isPaneOpenedOrClosedFromCode;

    public MainWindow(
        MainWindowViewModel viewModel,
        IServiceProvider serviceProvider,
        INavigationService navigationService,
        ISnackbarService snackbarService,
        IContentDialogService contentDialogService)
    {
        SystemThemeWatcher.Watch(this);

        ViewModel = viewModel;
        DataContext = this;

        InitializeComponent();

        var settings = App.GetRequiredService<ISettingsService>();
        ApplicationThemeManager.Apply(settings.Settings.Theme, WindowBackdropType.Mica, true, true);

        navigationService.SetNavigationControl(NavigationView);
        snackbarService.SetSnackbarPresenter(SnackbarPresenter);
        contentDialogService.SetContentPresenter(RootContentDialog);

        NavigationView.SetServiceProvider(serviceProvider);

        ApplicationThemeManager.Changed += OnThemeChanged;
    }

    public MainWindowViewModel ViewModel { get; set; } = null!;

    private void OnNavigationSelectionChanged(object sender, RoutedEventArgs e)
    {
        if (sender is not NavigationView navigationView)
        {
            return;
        }

        PageTitle.Text = navigationView.SelectedItem?.Content.ToString();

        NavigationView.HeaderVisibility =
            navigationView.SelectedItem?.TargetPageType == typeof(AudioLibraryPage) ||
            navigationView.SelectedItem?.TargetPageType == typeof(SettingsPage)
                ? Visibility.Visible
                : Visibility.Collapsed;
    }

    private void OnThemeChanged(ApplicationTheme currentApplicationTheme, Color systemAccent)
    {
        ApplyNavigationMenuIcons();
    }

    private void OnSizeChanged(object sender, SizeChangedEventArgs e)
    {
        if (_isUserClosedPane)
        {
            return;
        }

        _isPaneOpenedOrClosedFromCode = true;
        NavigationView.IsPaneOpen = !(e.NewSize.Width <= 1000);
        _isPaneOpenedOrClosedFromCode = false;
    }

    private void NavigationView_OnPaneOpened(NavigationView sender, RoutedEventArgs args)
    {
        if (_isPaneOpenedOrClosedFromCode)
        {
            return;
        }

        _isUserClosedPane = false;
    }

    private void NavigationView_OnPaneClosed(NavigationView sender, RoutedEventArgs args)
    {
        if (_isPaneOpenedOrClosedFromCode)
        {
            return;
        }

        _isUserClosedPane = true;
    }

    private void OnLoaded(object sender, RoutedEventArgs e)
    {
        ApplyNavigationMenuIcons();

        var immersivePlayerService = App.GetRequiredService<IImmersivePlayerService>();

        immersivePlayerService.SetPlayer(ImmersivePlayer);

        var dynamicIsland = App.GetRequiredService<DynamicIsland>();
        dynamicIsland.Show();

        AutoSuggest.TextChanged += OnAutoSuggestTextChanged;
        AutoSuggest.SuggestionChosen += OnSuggestionChosen;
        AutoSuggest.QuerySubmitted += OnAutoSuggestQuerySubmitted;
    }

    private async void OnAutoSuggestTextChanged(AutoSuggestBox sender, AutoSuggestBoxTextChangedEventArgs args)
    {
        await ViewModel.OnAutoSuggestTextChanged(args);
    }

    private async void OnAutoSuggestQuerySubmitted(AutoSuggestBox sender, AutoSuggestBoxQuerySubmittedEventArgs args)
    {
        await ViewModel.OnAutoSuggestQuerySubmitted(args);
    }

    private async void OnSuggestionChosen(AutoSuggestBox sender, AutoSuggestBoxSuggestionChosenEventArgs args)
    {
        await ViewModel.OnAutoSuggestChosen(args);
    }

    private void OnClosing(object sender, CancelEventArgs e)
    {
        Application.Current.Shutdown();
    }

    private void ApplyNavigationMenuIcons()
    {
        foreach (var item in NavigationView.MenuItems.Cast<NavigationViewItem>())
        {
            var iconSource = (ImageSource)Application.Current.Resources[item.TargetPageTag];

            if (iconSource is not null)
            {
                item.Icon = new ImageIcon()
                {
                    Source = iconSource,
                    Width = 24,
                    Height = 24,
                };
            }
        }
    }

    private static readonly Action _toggleProcess = () => App.GetRequiredService<ProcessMessageWindow>().ToggleProcess();

    private void OnAutoSuggestGotFocus(object sender, RoutedEventArgs e)
    {
        _toggleProcess();
    }

    private void OnAutoSuggestLostFocus(object sender, RoutedEventArgs e)
    {
        _toggleProcess();
    }
}