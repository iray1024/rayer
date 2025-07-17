using Rayer.Core.Models;

namespace Rayer.Core.Abstractions;

public interface ICoverManager
{
    Task SetCoverAsync(Audio audio, string mediaSource);

    Task RemoveCoverAsync(Audio audio);

    string? GetCover(Audio audio);

    event EventHandler<Audio> CoverChanged;
}