namespace Rayer.SearchEngine.Core.Domain.Search;

public record SearchSuggestDetail
{
    public long Id { get; set; }

    public string Name { get; set; } = string.Empty;
}