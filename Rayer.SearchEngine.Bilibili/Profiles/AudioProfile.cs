using AutoMapper;
using Rayer.SearchEngine.Bilibili.Models.Search;
using Rayer.SearchEngine.Core.Domain.Aduio;

namespace Rayer.SearchEngine.Bilibili.Profiles;

internal class AudioProfile : Profile
{
    public AudioProfile()
    {
        CreateMap<WebAudioModel, WebAudio>()
            .ForMember(d => d.Id, o => o.MapFrom(s => s.Data.Dash.Audio[0].Id))
            .ForMember(d => d.Url, o => o.MapFrom(s => s.Data.Dash.Audio[0].BaseUrl));
    }
}