namespace Rayer.SearchEngine.Models.Response.Login.User;

public record AccountDetail
{
    public long Id { get; set; }

    public int Type { get; set; } = 0;

    public int Status { get; set; } = -10;

    public int VipType { get; set; } = 0;

    public bool AnonimousUser { get; set; } = true;

    public bool PaidFee { get; set; } = false;
}