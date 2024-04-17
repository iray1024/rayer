using MaterialDesignThemes.Wpf;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Win32;
using Rayer.Installer.Abstractions;
using Rayer.Installer.Services;
using Rayer.Installer.ViewModels;
using System.Diagnostics;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media.Animation;
using Application = System.Windows.Application;

namespace Rayer.Installer;

public partial class MainWindow : Window
{
    private readonly IResourceExtractor _extractor;

    public MainWindow()
    {
        InitializeComponent();

        var vm = App.Services.GetRequiredService<MainViewModel>();

        ViewModel = vm;
        DataContext = this;

        vm.Progress.ProgressChanged += OnProgressChanged;

        _extractor = App.Services.GetRequiredService<IResourceExtractor>();

        StartPage.BtnInstall.Click += OnInstallClick;
        EndPage.BtnComplete.Click += OnCompleteClick;
    }

    public MainViewModel ViewModel { get; set; }

    private void OnInstallClick(object sender, RoutedEventArgs e)
    {
        if (ViewModel.IsInstallStart)
        {
            return;
        }

        ViewModel.IsInstallStart = true;
        ProgressBar.Visibility = Visibility.Visible;
        EndPage.BtnComplete.IsEnabled = false;
        BtnClose.IsEnabled = false;

        if (!EnsureDotNetRuntimeIsReady())
        {
            DialogHost.Show(RuntimeForce, "RootDialog");

            ViewModel.IsInstallStart = false;
            ProgressBar.Visibility = Visibility.Collapsed;
            EndPage.BtnComplete.IsEnabled = true;
            BtnClose.IsEnabled = true;

            Transitioner.SelectedIndex = -1;

            return;
        }

        Task.Run(async () =>
        {
            foreach (var map in Constants.Resources)
            {
                map.ResourceStream = _extractor.GetResource(map.Path);
            }

            await FileOperator.ExtractorAll(Constants.Resources, ViewModel.Progress);

            var rayerExe = Path.Combine(ViewModel.InstallPath, "Rayer.exe");

            WindowsUtils.CreateShortcutOnDesktop("Rayer", rayerExe, "喵蛙王子丶的音乐播放器", rayerExe);
            WindowsUtils.AddToStartMenu("Rayer", rayerExe, "喵蛙王子丶的音乐播放器", rayerExe);

            using var lastKey = Registry.CurrentUser.OpenSubKey(Constants.RegisterKey, true);

            if (lastKey is not null)
            {
                lastKey.SetValue("Install Path", ViewModel.InstallPath);
            }
            else
            {
                using var key = Registry.CurrentUser.CreateSubKey(Constants.RegisterKey);

                key.SetValue("Install Path", ViewModel.InstallPath);
            }
        }).ContinueWith(task =>
        {
            Application.Current.Dispatcher.InvokeAsync(async () =>
            {
                StartPage.BtnInstall.ToolTip = "已安装";
                EndPage.BtnComplete.IsEnabled = true;
                BtnClose.IsEnabled = true;

                var doubleAnimation = new DoubleAnimation(1.0, 0, TimeSpan.FromSeconds(1));
                ProgressBar.BeginAnimation(OpacityProperty, doubleAnimation);

                await Task.Delay(1000);

                ProgressBar.Visibility = Visibility.Collapsed;
            });
        });
    }

    private void OnCompleteClick(object sender, RoutedEventArgs e)
    {

        ClearResource();

    }

    private void OnProgressChanged(object? sender, double e)
    {
        ProgressBar.Value = e;
    }

    private void OnColorZoneMouseDown(object sender, MouseButtonEventArgs e)
    {
        if (e.LeftButton == MouseButtonState.Pressed)
        {
            DragMove();
        }
    }

    private void OnCloseClick(object sender, RoutedEventArgs e)
    {
        ClearResource();
    }

    private void ClearResource()
    {
        var tempPath = Path.Combine(Constants.InstallerTempDir);

        if (Directory.Exists(tempPath))
        {
            Directory.Delete(tempPath, true);
        }

        Close();
    }

    private bool EnsureDotNetRuntimeIsReady()
    {
        var process = new Process();
        process.StartInfo.FileName = "dotnet";
        process.StartInfo.Arguments = "--list-runtimes";
        process.StartInfo.RedirectStandardOutput = true;
        process.StartInfo.UseShellExecute = false;
        process.StartInfo.CreateNoWindow = true;
        process.Start();
        var output = process.StandardOutput.ReadToEnd();
        process.WaitForExit();

        var installed = output.Contains("Microsoft.WindowsDesktop.App 8");

        return installed;
    }

    private void OnNavigateToDownloadClick(object sender, RoutedEventArgs e)
    {
        Process.Start(new ProcessStartInfo(DotnetDownload.NavigateUri.AbsoluteUri)
        {
            UseShellExecute = true
        });
    }
}