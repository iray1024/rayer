using Rayer.Core.Framework.Injection;
using Rayer.SearchEngine.Core.Business.Lyric;
using Rayer.SearchEngine.Netease.Engine;
using Rayer.SearchEngine.Netease.Models.Lyric;

namespace Rayer.SearchEngine.Netease.Business.Lyric;

[Inject<ISpecialLyricService>]
internal class SpecialLyricService : SearchEngineBase, ISpecialLyricService
{
    public SpecialLyricService()
    {
    }

    public async Task<Core.Domain.Common.Lyric> GetLyricAsync(long id)
    {
        var result = await Searcher.GetAsync(
            LyricSelector.GetLyric()
                .WithParam("id", id.ToString())
                .Build());

        var response = result.ToEntity<LyricModel>();

        if (response is not null)
        {
            var domain = Mapper.Map<Core.Domain.Common.Lyric>(response);

            return domain;
        }

        return default!;
    }

    public Task GetLyricExAsync(long id)
    {
        throw new NotImplementedException();
    }
}