using Downloader;
using Rayer.Updater.Models;

namespace Rayer.Updater.Services;

public interface IUpdateService
{
    string[] Args { get; }

    void Initialize(string[] args);

    /// <summary>
    /// 获取最新的发布
    /// </summary>
    Task<Release> GetLatestReleaseAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// 获取本地App的版本号
    /// </summary>
    Task<AppVersion> GetLocalVersionAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// 更新
    /// </summary>
    Task<(DownloadService, string)> UpdateAsync(Release release, CancellationToken cancellationToken = default);
}