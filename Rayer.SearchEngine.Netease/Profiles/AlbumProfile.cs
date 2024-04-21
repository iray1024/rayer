using AutoMapper;
using Rayer.SearchEngine.Core.Domain.Album;
using Rayer.SearchEngine.Netease.Models;

namespace Rayer.SearchEngine.Netease.Profiles;

internal class AlbumProfile : Profile
{
    public AlbumProfile()
    {
        CreateMap<AlbumModel, AlbumDefinition>();
    }
}