namespace Rayer.SearchEngine.Core.Business.Lyric;
using Rayer.SearchEngine.Core.Domain.Common;

public interface ISpecialLyricService
{
    Task<Lyric> GetLyricAsync(long id);

    Task GetLyricExAsync(long id);
}