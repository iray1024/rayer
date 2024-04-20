using Rayer.Core.Framework.Injection;
using Rayer.SearchEngine.Business.Lyric.Abstractions;
using Rayer.SearchEngine.Extensions;
using Rayer.SearchEngine.Internal.Abstractions;
using Rayer.SearchEngine.Models.Response.Netease.Lyric;

namespace Rayer.SearchEngine.Business.Lyric.Impl;

[Inject<ISpecialLyricService>]
internal class SpecialLyricService : SearchEngineBase, ISpecialLyricService
{
    public SpecialLyricService(IServiceProvider serviceProvider) : base(serviceProvider)
    {
    }

    public async Task<LyricResponse> GetLyricAsync(long id)
    {
        var result = await Searcher.GetAsync(
            Lyric.GetLyric()
                .WithParam("id", id.ToString())
                .Build());

        var response = result.ToEntity<LyricResponse>();

        return response is not null ? response : default!;
    }

    public Task GetLyricExAsync(long id)
    {
        throw new NotImplementedException();
    }
}