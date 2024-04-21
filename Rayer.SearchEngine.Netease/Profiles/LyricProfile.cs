using AutoMapper;
using Rayer.SearchEngine.Core.Domain.Common;
using Rayer.SearchEngine.Netease.Models.Lyric;

namespace Rayer.SearchEngine.Netease.Profiles;

internal class LyricProfile : Profile
{
    public LyricProfile()
    {
        CreateMap<LyricModel.LyricDetailModel, Lyric.LyricDetailModel>();

        CreateMap<LyricModel, Lyric>();
    }
}