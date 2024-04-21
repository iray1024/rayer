using AutoMapper;
using Rayer.SearchEngine.Core.Domain.Aduio;
using Rayer.SearchEngine.Core.Domain.Search;
using Rayer.SearchEngine.Netease.Models.Search;

namespace Rayer.SearchEngine.Netease.Profiles;

internal class SearchProfile : Profile
{
    public SearchProfile()
    {
        CreateMap<SearchAudioDetailInformationModel, SearchSuggestDetail>();
        CreateMap<SearchAlbumDetailInformationModel, SearchSuggestDetail>();

        CreateMap<SearchSuggestModel, SearchSuggest>()
            .ForMember(d => d.Audios, o => o.MapFrom(s => s.Result.Audios))
            .ForMember(d => d.Albums, o => o.MapFrom(s => s.Result.Albums));

        CreateMap<SearchAudioDetailInformationModel, SearchAudioDetail>()
            .ForMember(d => d.Title, o => o.MapFrom(s => s.Name))
            .ForMember(d => d.Duration, o => o.MapFrom(s => TimeSpan.FromMilliseconds(s.Duration)))
            .ForMember(d => d.Rank, o => o.MapFrom(s => s.Pop));

        CreateMap<SearchAudioModel, SearchAudio>()
            .ForMember(d => d.Details, o => o.MapFrom(s => s.Result.Songs))
            .ForMember(d => d.Total, o => o.MapFrom(s => s.Result.Count));
    }
}