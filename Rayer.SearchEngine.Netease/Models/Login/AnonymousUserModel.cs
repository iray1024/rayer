using Rayer.SearchEngine.Core.Http.Abstractions;

namespace Rayer.SearchEngine.Netease.Models.Login;

public class AnonymousUserModel : ResponseBase
{
    public long UserId { get; set; }

    public long CreateTime { get; set; }

    public string Cookie { get; set; } = string.Empty;
}