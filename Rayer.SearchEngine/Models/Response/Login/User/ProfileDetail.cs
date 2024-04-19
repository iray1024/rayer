namespace Rayer.SearchEngine.Models.Response.Login.User;

public record ProfileDetail
{
    public long UserId { get; set; }

    public string NickName { get; set; } = string.Empty;

    [JsonPropertyName("avatarUrl")]
    public string Avatar { get; set; } = string.Empty;

    [JsonPropertyName("backgroundUrl")]
    public string Background { get; set; } = string.Empty;

    public string Signature { get; set; } = string.Empty;

    public long Birthday { get; set; }

    public int Gender { get; set; }
}