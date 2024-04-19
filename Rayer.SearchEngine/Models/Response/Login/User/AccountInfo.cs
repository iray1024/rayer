namespace Rayer.SearchEngine.Models.Response.Login.User;

public class AccountInfo : ResponseBase
{
    public AccountDetail Account { get; set; } = new();

    public ProfileDetail? Profile { get; set; }
}