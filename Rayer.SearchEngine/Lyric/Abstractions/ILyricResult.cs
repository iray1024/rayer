using Rayer.Core.Lyric.Enums;
using Rayer.SearchEngine.Lyric.Models;

namespace Rayer.SearchEngine.Lyric.Abstractions;

public interface ILyricResult
{
    public LyricWrapper GetLyric();

    public (string, LyricRawType) GetLyricTarget();
}