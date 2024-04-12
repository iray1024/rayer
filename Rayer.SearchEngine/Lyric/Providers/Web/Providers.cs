using Rayer.Core;

namespace Rayer.SearchEngine.Lyric.Providers.Web;

internal static class Providers
{
    private static QQMusic.Api? _qqMusicApi;

    public static QQMusic.Api QQMusicApi => _qqMusicApi ??= AppCore.GetRequiredService<QQMusic.Api>();

    private static Netease.Api? _neteaseApi;

    public static Netease.Api NeteaseApi => _neteaseApi ??= AppCore.GetRequiredService<Netease.Api>();

    private static Kugou.Api? _kugouApi;

    public static Kugou.Api KugouApi => _kugouApi ??= AppCore.GetRequiredService<Kugou.Api>();
}