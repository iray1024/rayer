namespace Rayer.SearchEngine.Models.Response.Login.User;

public class AccountInfoResponse : ResponseBase
{
    public AccountDetail Account { get; set; } = default!;

    public ProfileDetail Profile { get; set; } = default!;
}

public record AccountDetail
{
    public string Id { get; set; } = string.Empty;
}

public record ProfileDetail
{
    public string UserId { get; set; } = string.Empty;

    public string NickName { get; set; } = string.Empty;

    [JsonPropertyName("avatarUrl")]
    public string Avatar { get; set; } = string.Empty;

    [JsonPropertyName("backgroundUrl")]
    public string Background { get; set; } = string.Empty;

    public string Signature { get; set; } = string.Empty;
}