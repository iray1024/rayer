namespace Rayer.SearchEngine.Core.Domain.Aduio;

public record SearchAudio
{
    public SearchAudioDetail[] Details { get; set; } = null!;

    public int Page { get; set; }

    public int PageSize { get; set; }

    public int Total { get; set; }

    public bool HasMore => Page * PageSize < Total;

    public override int GetHashCode()
    {
        return HashCode.Combine(Details, Page, PageSize, Total);
    }
}