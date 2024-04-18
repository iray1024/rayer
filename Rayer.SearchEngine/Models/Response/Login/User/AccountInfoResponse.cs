namespace Rayer.SearchEngine.Models.Response.Login.User;

public class AccountInfoResponse : ResponseBase
{
    public AccountDetail Account { get; set; } = default!;

    public ProfileDetail? Profile { get; set; }
}

public record AccountDetail
{
    public long Id { get; set; }

    public int Type { get; set; }

    public int Status { get; set; }

    public int VipType { get; set; }

    public bool AnonimousUser { get; set; } = true;

    public bool PaidFee { get; set; } = false;
}

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