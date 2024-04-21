using AutoMapper;
using Rayer.SearchEngine.Core.Domain.Artist;
using Rayer.SearchEngine.Netease.Models;

namespace Rayer.SearchEngine.Netease.Profiles;

internal class ArtistProfile : Profile
{
    public ArtistProfile()
    {
        CreateMap<ArtistModel, ArtistDefinition>();
    }
}