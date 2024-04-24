using Rayer.SearchEngine.Core.Domain.Artist;
using Rayer.SearchEngine.Core.Domain.Authority;
using Rayer.SearchEngine.Netease.Models;
using Profile = Rayer.SearchEngine.Core.Domain.Authority.Profile;

namespace Rayer.SearchEngine.Netease.Profiles;

internal class ArtistProfile : AutoMapper.Profile
{
    public ArtistProfile()
    {
        CreateMap<ArtistModel, ArtistDefinition>();

        CreateMap<ArtistDefinition, User>()
            .ForMember(d => d.Account, o => o.MapFrom(s => new Account { Id = s.Id }))
            .ForMember(d => d.Profile, o => o.MapFrom(s => new Profile { Name = s.Name, Avatar = s.Cover }));
    }
}