using Rayer.SearchEngine.Models.Response.Search;

namespace Rayer.SearchEngine.Business.Search.Abstractions;

public interface ISearchAudioEngine
{
    Task<SearchAudioResposne> SearchAsync(string keywords, int offset);

    Task<SearchAudioDetailResponse> SearchDetailAsync(string ids);
}