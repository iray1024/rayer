using Rayer.SearchEngine.Core.Http.Abstractions;

namespace Rayer.SearchEngine.Netease.Models.Search.Album;

public class SearchAlbumSuggestInformationModel : ResponseBase
{
    public long Id { get; set; }

    public string Name { get; set; } = string.Empty;
}