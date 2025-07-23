namespace Rayer.Core.Models;

internal sealed class ShuffleLink
{
    public ShuffleLinkNode Current { get; set; } = null!;
}

internal sealed class ShuffleLinkNode(Audio audio)
{
    public ShuffleLinkNode? Previous { get; set; }

    public ShuffleLinkNode? Next { get; set; }

    public Audio Audio { get; set; } = audio;
}