namespace Rayer.SearchEngine.Bilibili.Http;

internal static class ApiEndpoints
{
    internal static class Search
    {
        internal const string SearchBvId = "https://api.bilibili.com/x/web-interface/search/type?__refresh__=true&_extra=&context=&page=1&page_size=42&platform=pc&highlight=1&single_column=0&keyword={0}&category_id=&search_type=video&dynamic_offset=0&preload=true&com2co=true";

        internal const string SearchCId = "https://api.bilibili.com/x/web-interface/view?bvid={0}";

        internal const string SearchUrl = "https://api.bilibili.com/x/player/playurl?fnval=16&bvid={0}&cid={1}";
    }
}