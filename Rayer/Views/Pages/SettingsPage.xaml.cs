using Rayer.Abstractions;
using Rayer.Core;
using Rayer.Core.Controls;
using Rayer.Core.Utils;
using Rayer.Services;
using Rayer.ViewModels;
using System.Diagnostics;
using System.Diagnostics.Contracts;
using System.IO;
using System.Windows;
using System.Windows.Media;
using Wpf.Ui;
using Wpf.Ui.Abstractions.Controls;
using Wpf.Ui.Appearance;
using Wpf.Ui.Controls;
using Wpf.Ui.Extensions;

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

    private void OnLoaded(object sender, RoutedEventArgs e)
    {
        var navView = AppCore.GetRequiredService<INavigationService>().GetNavigationControl() as NavigationView;

        if (navView?.Template.FindName("PART_NavigationViewContentPresenter", navView) is NavigationViewContentPresenter navPresenter)
        {
            var scrollViewer = ElementHelper.GetScrollViewer(navPresenter);

            scrollViewer?.ScrollToTop();
        }
    }

    private async void OnAboutClicked(object sender, RoutedEventArgs e)
    {
        var contentDialogService = AppCore.GetRequiredService<IContentDialogService>();
        var updater = AppCore.GetRequiredService<IUpdateService>();

        var localPath = Directory.GetCurrentDirectory();
        var fileVersionInfo = FileVersionInfo.GetVersionInfo(Path.Combine(localPath, "rayer.exe"));

        Contract.Assert(fileVersionInfo is { FileVersion: not null });

        var local = Version.Parse(fileVersionInfo.FileVersion);

        var dialog = new AboutContentDialog(contentDialogService.GetDialogHost())
        {
            Title = "关于 Rayer",
            IsFooterVisible = false,
            UpdateImpl = async () =>
            {
                await Application.Current.Dispatcher.InvokeAsync(async () =>
                {
                    var updater = AppCore.GetRequiredService<IUpdateService>();
                    if (await updater.CheckUpdateAsync(AppCore.StoppingToken))
                    {
                        await updater.UpdateAsync(AppCore.StoppingToken);
                    }
                    else
                    {
                        var dialogService = AppCore.GetRequiredService<IContentDialogService>();
                        await dialogService.ShowSimpleDialogAsync(new SimpleContentDialogCreateOptions
                        {
                            Title = "恭喜",
                            Content = "您已是最新版本！",
                            CloseButtonText = "关闭"
                        });
                    }
                });
            }
        };

        AboutContentDialog.SetLogo(dialog, ImageSourceFactory.Create("pack://application:,,,/assets/logo.png"));
        AboutContentDialog.SetDescription(dialog, $"Rayer {local.ToString(3)}\n喵蛙王子丶 版权所有\nCopyright(C) 2020-2025 MM. All Rights Reserved");

        await dialog.ShowAsync();
    }
}