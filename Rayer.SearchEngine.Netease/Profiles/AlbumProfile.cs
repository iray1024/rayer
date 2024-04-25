using AutoMapper;
using Rayer.SearchEngine.Core.Domain.Aduio;
using Rayer.SearchEngine.Core.Domain.Album;
using Rayer.SearchEngine.Core.Domain.Artist;
using Rayer.SearchEngine.Netease.Models;
using Rayer.SearchEngine.Netease.Models.Search.Album;

namespace Rayer.SearchEngine.Netease.Profiles;

internal class AlbumProfile : Profile
{
    public AlbumProfile()
    {
        CreateMap<AlbumModel, AlbumDefinition>()
            .ForMember(d => d.Title, o => o.MapFrom(s => s.Name));

        CreateMap<FavAlbumModel, Album[]>()
            .ConvertUsing(DetailConverter);

        CreateMap<AlbumDetailModel, Album>()
            .ConvertUsing(DetailsConverter);
    }

    private readonly Func<AlbumDetailModel, Album, ResolutionContext, Album> DetailsConverter =
        (source, dest, ctx) => new Album
        {
            Id = source.Album.Id,
            Title = source.Album.Name,
            Cover = source.Album.Cover,
            Artist = ctx.Mapper.Map<ArtistDefinition>(source.Album.Artist),
            PublishTime = DateTimeOffset.FromUnixTimeMilliseconds(source.Album.PublishTime).DateTime,
            Description = source.Album.Description,
            TotalMinutes = (int)TimeSpan.FromMilliseconds(double.Parse(source.Audios.Sum(x => x.Duration).ToString())).TotalMinutes,
            Company = source.Album.Company,
            AudioCount = source.Audios.Length,
            Audios = ctx.Mapper.Map<SearchAudioDetail[]>(source.Audios)
        };

    private readonly Func<FavAlbumModel, Album[], ResolutionContext, Album[]> DetailConverter =
        (source, dest, ctx) => source.Data.Select(x => new Album
        {
            Id = x.Id,
            Title = x.Name,
            Cover = x.Cover,
            AudioCount = x.Size,
            Artist = ctx.Mapper.Map<ArtistDefinition>(x.Artists[0])
        }).ToArray();
}