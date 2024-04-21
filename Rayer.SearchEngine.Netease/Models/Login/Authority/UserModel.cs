using Rayer.SearchEngine.Core.Http.Abstractions;

namespace Rayer.SearchEngine.Netease.Models.Login.Authority;

public class UserModel : ResponseBase
{
    public AccountModel Account { get; set; } = new();

    public ProfileModel? Profile { get; set; }
}