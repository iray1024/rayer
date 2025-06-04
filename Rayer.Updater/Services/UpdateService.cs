using Downloader;
using Rayer.Core.Framework.Injection;
using Rayer.Updater.Models;
using System.Diagnostics;
using System.Diagnostics.Contracts;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Net.Mime;

namespace Rayer.Updater.Services;

[Inject<IUpdateService>]
internal sealed class UpdateService : IUpdateService
{
    private const string GITHUB_TOKEN = "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";

    public string[] Args { get; set; } = [];

    public async Task<Release> GetLatestReleaseAsync(CancellationToken cancellationToken = default)
    {
        using var http = new HttpClient();
        http.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", GITHUB_TOKEN);
        http.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/vnd.github+json"));
        http.DefaultRequestHeaders.Add("X-GitHub-Api-Version", "2022-11-28");
        http.DefaultRequestHeaders.Host = "api.github.com";
        http.DefaultRequestHeaders.UserAgent.Add(new ProductInfoHeaderValue("iray1024", "1.0"));

        var release = await http.GetFromJsonAsync<Release>("https://api.github.com/repos/iray1024/rayer/releases/latest", cancellationToken: cancellationToken);

        Contract.Assert(release is not null);

        release.Version = Version.Parse(release.Tag.Replace("v", string.Empty, StringComparison.OrdinalIgnoreCase));

        return release;
    }

    public Task<AppVersion> GetLocalVersionAsync(CancellationToken cancellationToken = default)
    {
        const string localPath = @"C:\Program Files\Rayer";
        Console.WriteLine(string.Join(',', Args));

        if (Args.Length == 0)
        {
            Args = [localPath];
        }

        Console.WriteLine(Args[0]);

        var fileVersionInfo = FileVersionInfo.GetVersionInfo(Path.Combine(Args[0], "rayer.exe"));

        Contract.Assert(fileVersionInfo is { FileVersion: not null });

        var version = Version.Parse(fileVersionInfo.FileVersion);
        var appVersion = new AppVersion
        {
            Version = version
        };

        return Task.FromResult(appVersion);
    }

    public async Task<(DownloadService, string)> UpdateAsync(Release release, CancellationToken cancellationToken = default)
    {
        using var http = new HttpClient();
        http.Timeout = TimeSpan.FromMinutes(5);
        http.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", GITHUB_TOKEN);
        http.DefaultRequestHeaders.Add("X-GitHub-Api-Version", "2022-11-28");
        http.DefaultRequestHeaders.Host = "github.com";
        http.DefaultRequestHeaders.UserAgent.Add(new ProductInfoHeaderValue("iray1024", "1.0"));
        http.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue(MediaTypeNames.Application.Octet));

        var response = await http.GetAsync(release.Assets[0].DownloadUrl, cancellationToken);
        if (response.StatusCode is HttpStatusCode.InternalServerError)
        {
            var newUrl = response.RequestMessage!.RequestUri!.OriginalString;
            var uri = new Uri(newUrl);

            var downloadOptions = new DownloadConfiguration()
            {
                ChunkCount = 8,
                ParallelDownload = true,
                BufferBlockSize = 10240,
                MaxTryAgainOnFailover = 5,
                ParallelCount = 4,
                Timeout = 5000,
                RequestConfiguration =
                {
                    Accept = MediaTypeNames.Application.Octet,
                    UserAgent = "iray1024/1.0",
                    Headers = [$"Authorization: Bearer {GITHUB_TOKEN}", "X-GitHub-Api-Version: 2022-11-28", $"Host: {uri.Host}"],
                    Proxy = WebRequest.DefaultWebProxy
                }
            };

            return (new DownloadService(downloadOptions), newUrl);
        }

        return (default!, string.Empty);
    }
}