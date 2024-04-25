using Rayer.SearchEngine.Core.Domain.Album;
using System.ComponentModel;

namespace Rayer.SearchEngine.Core.Abstractions;

[EditorBrowsable(EditorBrowsableState.Never)]
public interface ISearchAlbumEngine
{
    Task<SearchAlbum> SearchAsync(string keywords, int offset);

    Task<Album> SearchAlbumDetailAsync(long id);
}