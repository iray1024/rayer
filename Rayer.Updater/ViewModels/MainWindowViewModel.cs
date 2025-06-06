using CommunityToolkit.Mvvm.ComponentModel;
using Downloader;
using ICSharpCode.SharpZipLib.Zip;
using Rayer.FrameworkCore;
using Rayer.FrameworkCore.Injection;
using Rayer.Updater.Models;
using Rayer.Updater.Services;
using System.Diagnostics;
using System.IO;
using System.Windows;
using Wpf.Ui;
using Wpf.Ui.Extensions;

namespace Rayer.Updater.ViewModels;

[Inject]
public partial class MainWindowViewModel(IUpdateService updateService) : ObservableObject
{
    [ObservableProperty]
    private string _localVersion = default!;

    [ObservableProperty]
    private string _latestVersion = default!;

    [ObservableProperty]
    private double _percent;

    [ObservableProperty]
    private DownloadInfo _downloadInfo = new();

    [ObservableProperty]
    private double? _downloadSpeed;

    [ObservableProperty]
    private bool _finished;

    [ObservableProperty]
    private string? _workingDirectory;

    public async Task CheckUpdateAsync(CancellationToken cancellationToken = default)
    {
        WorkingDirectory = $"工作目录: {updateService.Args[0]}";

        var local = await updateService.GetLocalVersionAsync(AppCore.StoppingToken);
        var latest = await updateService.GetLatestReleaseAsync(AppCore.StoppingToken);

        LocalVersion = local.ToString(3);
        LatestVersion = latest.Version.ToString(3);

        var dialogService = AppCore.GetRequiredService<IContentDialogService>();
        var needUpdate = latest.Version > local;
        if (needUpdate)
        {
            var (downloader, url) = await updateService.UpdateAsync(latest, cancellationToken);

            downloader.DownloadStarted += OnDownloadStarted;
            downloader.DownloadProgressChanged += OnDownloadProgressChanged;

            var webStream = await downloader.DownloadFileTaskAsync(url, cancellationToken);

            webStream.Position = 0;

            var installedDir = updateService.Args[0];
            using var inStream = new ZipInputStream(webStream, StringCodec.FromCodePage(StringCodec.SystemDefaultCodePage));
            ZipEntry zipEntry;

            while ((zipEntry = inStream.GetNextEntry()) is not null)
            {
                var entryPath = Path.Combine(installedDir, zipEntry.Name);
                var entryDir = Path.GetDirectoryName(entryPath);

                var fileName = Path.GetFileName(zipEntry.Name);

                if (!string.IsNullOrEmpty(fileName))
                {
                    using var outStream = File.Create(entryPath);

                    try
                    {
                        var buffer = new byte[1024];
                        var length = 0;

                        while ((length = await inStream.ReadAsync(buffer, cancellationToken)) > 0)
                        {
                            await outStream.WriteAsync(buffer.AsMemory(0, length), cancellationToken);
                        }

                        await outStream.FlushAsync(cancellationToken);
                    }
                    finally
                    {
                        outStream.Close();
                    }
                }
            }

            await Application.Current.Dispatcher.InvokeAsync(async () =>
            {
                await dialogService.ShowSimpleDialogAsync(new SimpleContentDialogCreateOptions
                {
                    Title = "恭喜",
                    Content = "更新成功！",
                    CloseButtonText = "关闭"
                });

                Finished = true;
            });
        }
        else
        {
            await Application.Current.Dispatcher.InvokeAsync(async () =>
            {
                await dialogService.ShowSimpleDialogAsync(new SimpleContentDialogCreateOptions
                {
                    Title = "恭喜",
                    Content = "您已是最新版本！",
                    CloseButtonText = "关闭"
                });

                Finished = true;
            });
        }
    }

    private void OnDownloadProgressChanged(object? sender, DownloadProgressChangedEventArgs e)
    {
        Percent = Math.Round(e.ProgressPercentage, 2);
        DownloadSpeed = e.BytesPerSecondSpeed;

        DownloadInfo.ReceiveBytes = e.ReceivedBytesSize;

        OnPropertyChanged(nameof(DownloadInfo));
    }

    private void OnDownloadStarted(object? sender, DownloadStartedEventArgs e)
    {
        DownloadInfo.TotalBytes = e.TotalBytesToReceive;
    }

    partial void OnFinishedChanged(bool value)
    {
        Process.Start(Path.Combine(updateService.Args[0], "rayer.exe"));
        Application.Current.Shutdown();
    }
}