using AutoMapper;
using Rayer.SearchEngine.Core.Domain.Aduio;
using Rayer.SearchEngine.Core.Domain.Album;
using Rayer.SearchEngine.Core.Domain.Playlist;
using Rayer.SearchEngine.Netease.Models.Search.Playlist;
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

        CreateMap<Album, PlaylistDetail>()
            .ForMember(d => d.Creator, o => o.MapFrom(s => s.Artist))
            .ForMember(d => d.CreateTime, o => o.MapFrom(s => s.PublishTime));
    }

    private readonly Func<UserPlaylistModel, PlaylistDetail[], ResolutionContext, PlaylistDetail[]> DetailsConverter =
        (souce, dest, ctx) => souce.Playlist.Select(x => new PlaylistDetail
        {
            Id = x.Id,
            OwnerId = x.Creator.UserId,
            Title = x.Name,
            Creator = ctx.Mapper.Map<Core.Domain.Authority.User>(x.Creator),
            CreateTime = DateTimeOffset.FromUnixTimeMilliseconds(x.CreateTime).DateTime,
            UpdateTime = DateTimeOffset.FromUnixTimeMilliseconds(x.UpdateTime).DateTime,
            Cover = x.Cover,
            AudioCount = x.TrackCount,
            PlayCount = x.PlayCount,
            Audios = ctx.Mapper.Map<SearchAudioDetail[]>(x.Tracks)
        }).ToArray();

    private readonly Func<PlaylistDetailModel, PlaylistDetail, ResolutionContext, PlaylistDetail> DetailConverter =
        (souce, dest, ctx) => new PlaylistDetail
        {
            Id = souce.Playlist.Id,
            OwnerId = souce.Playlist.Creator.UserId,
            Title = souce.Playlist.Name,
            Creator = ctx.Mapper.Map<Core.Domain.Authority.User>(souce.Playlist.Creator),
            CreateTime = DateTimeOffset.FromUnixTimeMilliseconds(souce.Playlist.CreateTime).DateTime,
            UpdateTime = DateTimeOffset.FromUnixTimeMilliseconds(souce.Playlist.UpdateTime).DateTime,
            Cover = souce.Playlist.Cover,
            AudioCount = souce.Playlist.TrackCount,
            PlayCount = souce.Playlist.PlayCount,
            Audios = ctx.Mapper.Map<SearchAudioDetail[]>(souce.Playlist.Tracks)
        };
}