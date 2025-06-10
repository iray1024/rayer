namespace Rayer.Core.Models;

public readonly struct PlaybackRecord
{
    public string? Id { readonly get; init; } 

    public TimeSpan Offset { readonly get; init; }
}