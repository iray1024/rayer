namespace Rayer.SearchEngine.Core.Domain.Search;

public class SearchSuggest
{
    public int Code { get; set; }

    public SearchSuggestDetail[] Albums { get; set; } = [];

    public SearchSuggestDetail[] Audios { get; set; } = [];
}