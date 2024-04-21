namespace Rayer.SearchEngine.Core.Domain.Common;

public record Copyright
{
    public static readonly Copyright Default = new();

    public bool HasCopyright => string.IsNullOrEmpty(Reason);

    public string Reason { get; set; } = string.Empty;
}