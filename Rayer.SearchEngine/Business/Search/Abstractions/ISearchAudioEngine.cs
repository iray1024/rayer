using Rayer.SearchEngine.Models.Response.Netease.Search;

namespace Rayer.SearchEngine.Business.Search.Abstractions;

public interface ISearchAudioEngine
{
    Task<SearchAudio> SearchAsync(string keywords, int offset);

    Task<SearchAudioDetail> SearchDetailAsync(string ids);

    Task<WebAudio> GetAudioAsync(long id);
}