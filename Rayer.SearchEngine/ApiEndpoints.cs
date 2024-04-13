using System.ComponentModel;

namespace Rayer.SearchEngine;

internal static class ApiEndpoints
{
    internal static class Login
    {
        internal static string Captcha = "/captcha/sent";

        internal static string CaptchaVerify = "/captcha/verify";

        internal static string PhoneLogin = "/login/cellphone";

        internal static string QrCodeKey = "/login/qr/key";

        internal static string QrCodeCreate = "/login/qr/create";

        internal static string QrCodeVerify = "/login/qr/check";

        internal static string AnonymousLogin = "/register/anonimous";

        internal static string RefreshLogin = "/login/refresh";

        internal static string LoginStatus = "/login/status";
    }

    internal static class Account
    {
        internal static string UserDetail = "/user/detail";

        internal static string UserInfo = "/user/subcount";

        internal static string AccountInfo = "/user/account";
    }

    internal static class Track
    {
        internal static string GetTrack = "/song/url";

        internal static string GetTrackEx = "/song/url/v1";

        internal static string VerifyTrack = "/check/music";

        internal static string Download = "/song/download/url";

        [Description("获取歌曲详情")]
        internal static string TrackDetail = "/song/detail";

        [Description("获取音质详情")]
        internal static string TrackQualityDetail = "song/music/detail";
    }

    internal static class Search
    {
        internal static string SampleSearch = "/search";

        [Description("更全面的搜索")]
        internal static string CloudSearch = "/cloudsearch";

        internal static string HotSearch = "/search/hot";

        internal static string HotSearchDetail = "/search/hot/detail";

        internal static string SearchSuggestion = "/search/suggest";
    }

    internal static class Lyric
    {
        internal static string GetLyric = "/lyric";

        [Description("获取逐字歌词")]
        internal static string GetLyricEx = "/lyric/new";
    }
}