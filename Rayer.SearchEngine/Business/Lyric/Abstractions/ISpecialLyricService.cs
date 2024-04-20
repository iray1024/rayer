using Rayer.SearchEngine.Models.Response.Netease.Lyric;

namespace Rayer.SearchEngine.Business.Lyric.Abstractions;

public interface ISpecialLyricService
{
    Task<LyricResponse> GetLyricAsync(long id);

    Task GetLyricExAsync(long id);
}