using Rayer.Abstractions;
using Rayer.Core;
using Rayer.Core.Framework;
using Rayer.Core.Framework.Injection;
using Rayer.Core.Framework.Settings.Abstractions;
using Rayer.SearchEngine.Abstractions;
using Rayer.SearchEngine.Views.Windows;
using Rayer.ViewModels;
using Rayer.Views.Pages;
using System.ComponentModel;
using System.Windows;
using System.Windows.Input;
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
        IContentDialogService contentDialogService,
        ILoaderProvider loaderProvider)
    {
        SystemThemeWatcher.Watch(this);

        ViewModel = viewModel;
        DataContext = this;

        InitializeComponent();

        var settings = App.GetRequiredService<ISettingsService>();
        ApplicationThemeManager.Apply(settings.Settings.Theme, WindowBackdropType.Mica, true);

        navigationService.SetNavigationControl(NavigationView);
        snackbarService.SetSnackbarPresenter(SnackbarPresenter);
        contentDialogService.SetDialogHost(RootContentDialog);
        loaderProvider.SetLoader(Loader, 160, 0);

        NavigationView.SetServiceProvider(serviceProvider);

        ApplicationThemeManager.Changed += OnThemeChanged;
    }

    public MainWindowViewModel ViewModel { get; set; } = null!;

    private void OnNavigating(NavigationView sender, NavigatingCancelEventArgs args)
    {
        var pageType = args.Page.GetType();

        NavigationView.HeaderVisibility =
            pageType != typeof(AudioLibraryPage) &&
            pageType != typeof(SettingsPage)
                ? Visibility.Collapsed
                : Visibility.Visible;

        PageHeaderContainer.Visibility =
            pageType != typeof(AudioLibraryPage) &&
            pageType != typeof(SettingsPage)
                ? Visibility.Collapsed
                : Visibility.Visible;
    }

    private void OnNavigationSelectionChanged(object sender, RoutedEventArgs e)
    {
        if (sender is not NavigationView navigationView)
        {
            return;
        }

        PageHeader.Text = navigationView.SelectedItem?.Content.ToString();
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

    private void OnSuggestionChosen(AutoSuggestBox sender, AutoSuggestBoxSuggestionChosenEventArgs args)
    {
        ViewModel.OnAutoSuggestChosen(args);
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

    private async void OnAutoSuggestPreviewKeyDown(object sender, KeyEventArgs e)
    {
        if (AutoSuggest.IsSuggestionListOpen &&
            e.Key is Key.Enter)
        {
            await ViewModel.OnUserRaiseAutoSuggestChosen(AutoSuggest);
        }
    }
}