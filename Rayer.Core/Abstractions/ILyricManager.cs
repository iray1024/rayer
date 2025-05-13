using Rayer.Core.Models;

namespace Rayer.Core.Abstractions;

public interface ILyricManager
{
    void Store(Audio audio, int offset);

    int LoadOffset(Audio audio);
}