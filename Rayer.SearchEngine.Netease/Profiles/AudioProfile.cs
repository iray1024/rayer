using AutoMapper;
using Rayer.SearchEngine.Core.Domain.Aduio;
using Rayer.SearchEngine.Netease.Models.Search.Audio;

namespace Rayer.SearchEngine.Netease.Profiles;

internal class AudioProfile : Profile
{
    public AudioProfile()
    {
        CreateMap<WebAudioModel, WebAudio>()
            .ForMember(d => d.Id, o => o.MapFrom(s => s.Data[0].Id))
            .ForMember(d => d.Url, o => o.MapFrom(s => s.Data[0].Url));
    }
}