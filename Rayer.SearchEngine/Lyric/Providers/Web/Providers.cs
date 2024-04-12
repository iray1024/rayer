using Rayer.Core;

namespace Rayer.SearchEngine.Lyric.Providers.Web;

internal static class Providers
{
    private static Netease.Api? _neteaseApi;
    private static QQMusic.Api? _qqMusicApi;
    private static Kugou.Api? _kugouApi;

    public static Netease.Api NeteaseApi => _neteaseApi ??= AppCore.GetRequiredService<Netease.Api>();

    public static QQMusic.Api QQMusicApi => _qqMusicApi ??= AppCore.GetRequiredService<QQMusic.Api>();

    public static Kugou.Api KugouApi => _kugouApi ??= AppCore.GetRequiredService<Kugou.Api>();
}