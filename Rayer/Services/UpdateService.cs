using Rayer.Abstractions;
using Rayer.Core;
using Rayer.Core.Abstractions;
using Rayer.Core.Framework.Injection;
using System.Diagnostics;
using System.Diagnostics.Contracts;
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Windows;
using Wpf.Ui;
using Wpf.Ui.Controls;
using Wpf.Ui.Extensions;

namespace Rayer.Services;

[Inject<IUpdateService>]
internal sealed class UpdateService(IGitHubManager gitHubManager) : IUpdateService
{
    public async Task<bool> CheckUpdateAsync(CancellationToken cancellationToken = default)
    {
        using var http = new HttpClient();
        http.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", gitHubManager.Token);
        http.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/vnd.github+json"));
        http.DefaultRequestHeaders.Add("X-GitHub-Api-Version", "2022-11-28");
        http.DefaultRequestHeaders.Host = "api.github.com";
        http.DefaultRequestHeaders.UserAgent.Add(new ProductInfoHeaderValue("iray1024", "1.0"));

        var release = await http.GetFromJsonAsync<Release>("https://api.github.com/repos/iray1024/rayer/releases/latest", cancellationToken: cancellationToken);

        Contract.Assert(release is not null);

        release.Version = Version.Parse(release.Tag.Replace("v", string.Empty, StringComparison.OrdinalIgnoreCase));

        var localPath = Directory.GetCurrentDirectory();
        var fileVersionInfo = FileVersionInfo.GetVersionInfo(Path.Combine(localPath, "rayer.exe"));

        Contract.Assert(fileVersionInfo is { FileVersion: not null });

        var version = Version.Parse(fileVersionInfo.FileVersion);

        var dialogService = AppCore.GetRequiredService<IContentDialogService>();

        var result = await dialogService.ShowSimpleDialogAsync(new SimpleContentDialogCreateOptions
        {
            Title = "存在新版本",
            Content = $"当前版本: {version.ToString(3)}\n最新版本: {release.Version.ToString(3)}",
            CloseButtonText = "立即更新",
            PrimaryButtonText = "暂不更新",
        }, cancellationToken: cancellationToken);

        if (result is ContentDialogResult.None)
        {
            return true;
        }

        return false;
    }

    public Task UpdateAsync(CancellationToken cancellationToken = default)
    {
        //var updaterPath = @"C:\Users\mm\source\repos\rayer\Rayer.Updater\bin\Debug\net9.0-windows10.0.26100.0\Rayer.Updater.exe";
        var updaterPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "Rayer", "updater", "Rayer.Updater.exe");

        Process.Start(updaterPath, AppDomain.CurrentDomain.BaseDirectory);
        Application.Current.Shutdown();

        return Task.CompletedTask;
    }

    public class Release
    {
        [JsonPropertyName("tag_name")]
        public string Tag { get; set; } = null!;

        [JsonIgnore]
        public Version Version { get; set; } = default!;

        public Assets[] Assets { get; set; } = [];
    }

    public class Assets
    {
        public string Name { get; set; } = null!;

        public string Url { get; set; } = null!;

        [JsonPropertyName("browser_download_url")]
        public string DownloadUrl { get; set; } = null!;
    }
}