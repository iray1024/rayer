using Rayer.SearchEngine.Core.Http.Abstractions;

namespace Rayer.SearchEngine.Netease.Models.Search.Album;

internal class FavAlbumModel : ResponseBase
{
    public FavAlbumDetailModel[] Data { get; set; } = [];

    public int Count { get; set; }

    public bool HasMore { get; set; } = false;

    public long PaidCount { get; set; }
}