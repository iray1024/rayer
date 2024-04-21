using AutoMapper;
using Rayer.SearchEngine.Core.Domain.Aduio;
using Rayer.SearchEngine.Core.Domain.Playlist;
using Rayer.SearchEngine.Netease.Models.User;

namespace Rayer.SearchEngine.Netease.Profiles;

internal class PlaylistProfile : Profile
{
    public PlaylistProfile()
    {
        CreateMap<UserLikelistModel, PlaylistDetail>()
            .ForMember(d => d.Audios, o => o.MapFrom(s => s.Ids.Select(x => new SearchAudioDetail() { Id = x })));

        CreateMap<UserPlaylistModel, PlaylistDetail[]>()
            .ConvertUsing(DetailsConverter);

        CreateMap<PlaylistDetailModel, PlaylistDetail>()
            .ConvertUsing(DetailConverter);
    }

    private readonly Func<UserPlaylistModel, PlaylistDetail[], ResolutionContext, PlaylistDetail[]> DetailsConverter =
        (souce, dest, ctx) => souce.Playlist.Select(x => new PlaylistDetail
        {
            Id = x.Id,
            OwnerId = x.Creator.UserId,
            Title = x.Name,
            Creator = ctx.Mapper.Map<Core.Domain.Authority.User>(x.Creator),
            Cover = x.Cover,
            AudioCount = x.TrackCount,
            Audios = ctx.Mapper.Map<SearchAudioDetail[]>(x.Tracks)
        }).ToArray();

    private readonly Func<PlaylistDetailModel, PlaylistDetail, ResolutionContext, PlaylistDetail> DetailConverter =
        (souce, dest, ctx) => new PlaylistDetail
        {
            Id = souce.Playlist.Id,
            OwnerId = souce.Playlist.Creator.UserId,
            Title = souce.Playlist.Name,
            Creator = ctx.Mapper.Map<Core.Domain.Authority.User>(souce.Playlist.Creator),
            Cover = souce.Playlist.Cover,
            AudioCount = souce.Playlist.TrackCount,
            Audios = ctx.Mapper.Map<SearchAudioDetail[]>(souce.Playlist.Tracks)
        };
}