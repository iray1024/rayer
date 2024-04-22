using Rayer.SearchEngine.Core.Domain.Aduio;
using System.ComponentModel;

namespace Rayer.SearchEngine.Core.Abstractions;

[EditorBrowsable(EditorBrowsableState.Never)]
public interface ISearchAudioEngine
{
    Task<SearchAudio> SearchAsync(string keywords, int offset);

    Task<SearchAudioDetail[]> SearchDetailAsync(SearchAudioDetail[] details);

    Task<WebAudio> GetAudioAsync(SearchAudioDetail detail);
}