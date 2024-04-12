using Rayer.SearchEngine.Lyric.Models;

namespace Rayer.SearchEngine.Lyric.Abstractions;

public interface ILyricResult
{
    public LyricWrapper GetLyric();
}