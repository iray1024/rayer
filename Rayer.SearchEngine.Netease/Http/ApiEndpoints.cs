using System.ComponentModel;

namespace Rayer.SearchEngine.Netease.Http;

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

    internal static class User
    {
        /// <summary>
        /// 获取喜欢的音乐列表
        /// </summary>
        internal static string GetLikelist = "/likelist";

        /// <summary>
        /// 获取播放记录
        /// </summary>
        internal static string GetPlayRecord = "/user/record";

        /// <summary>
        /// 获取已收藏歌单列表
        /// </summary>
        internal static string GetFavPlaylist = "/user/playlist";

        /// <summary>
        /// 获取已收藏专辑列表
        /// </summary>
        internal static string GetFavAlbumlist = "/album/sublist";

        /// <summary>
        /// 获取已关注艺人列表
        /// </summary>
        internal static string GetFavArtistlist = "/artist/sublist";

        /// <summary>
        /// 获取已收藏MV列表
        /// </summary>
        internal static string GetFavMvlist = "/mv/sublist";

        /// <summary>
        /// 获取云盘列表
        /// </summary>
        internal static string GetCloudlist = "/user/cloud";
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

    internal static class Playlist
    {
        internal static string GetPlaylistDetail = "/playlist/detail";

        internal static string GetPlaylistAllAudio = "/playlist/track/all";
    }

    internal static class Album
    {
        internal static string GetAlbum = "/album";

        internal static string GetArtistAlbum = "/artist/album";
    }

    internal static class Lyric
    {
        internal static string GetLyric = "/lyric";

        [Description("获取逐字歌词")]
        internal static string GetLyricEx = "/lyric/new";
    }
}