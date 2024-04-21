namespace Rayer.SearchEngine.Netease.Models.Login.Authority;

public record ProfileModel
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