namespace Rayer.SearchEngine.Core.Domain.Search;

public class SearchSuggest
{
    public static readonly SearchSuggest Empty = new() { Code = -1 };

    public int Code { get; set; }

    public SearchSuggestDetail[] Albums { get; set; } = [];

    public SearchSuggestDetail[] Audios { get; set; } = [];
}