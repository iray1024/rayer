using Rayer.SearchEngine.Core.Http.Abstractions;

namespace Rayer.SearchEngine.Netease.Models.Search;

public class SearchAlbumDetailInformationModel : ResponseBase
{
    public long Id { get; set; }

    public string Name { get; set; } = string.Empty;
}