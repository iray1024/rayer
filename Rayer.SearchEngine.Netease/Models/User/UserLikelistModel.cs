using Rayer.SearchEngine.Core.Http.Abstractions;

namespace Rayer.SearchEngine.Netease.Models.User;

public class UserLikelistModel : ResponseBase
{
    public long[] Ids { get; set; } = [];

    public long CheckPoint { get; set; }
}