using Rayer.SearchEngine.Core.Enums;

namespace Rayer.SearchEngine.Core.Domain.Authority;

public record Profile
{
    public string Name { get; set; } = string.Empty;

    public string Avatar { get; set; } = string.Empty;

    public string Background { get; set; } = string.Empty;

    public string Signature { get; set; } = string.Empty;

    public DateOnly Birthday { get; set; }

    public Gender Gender { get; set; }
}