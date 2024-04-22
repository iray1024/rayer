using AutoMapper;
using Rayer.SearchEngine.Core.Domain.Aduio;
using Rayer.SearchEngine.Core.Domain.Album;
using Rayer.SearchEngine.Core.Domain.Artist;
using Rayer.SearchEngine.Models.Response.Bilibili.Search;
using System.Text.RegularExpressions;

namespace Rayer.SearchEngine.Bilibili.Profiles;

internal partial class SearchProfile : Profile
{
    public SearchProfile()
    {

        CreateMap<SearchAudioDetailInformationModel, SearchAudioDetail>()
            .ForMember(d => d.Title, o => o.MapFrom(s => RemoveHtmlTags().Replace(s.Title, "")))
            .ForMember(d => d.Duration, o => o.MapFrom(s => ParseTimeSpan(s.Duration)))
            .ForMember(d => d.Rank, o => o.MapFrom(s => s.RankScore))
            .ForMember(d => d.Artists, o => o.MapFrom(s => new ArtistDefinition[] { new()
            {
                Id = s.Aid,
                Name = s.Author,
                Picture = s.AuthorPicture
            }}))
            .ForMember(d => d.Album, o => o.MapFrom(s => new AlbumDefinition()
            {
                Id = s.Id,
                Title = s.TypeName,
                Picture = !s.Pic.StartsWith("http") ? $"https:{s.Pic}" : s.Pic
            }))
            .AfterMap((s, d) =>
            {
                d.Tags.Add("BvId", s.Bvid);
            });

        CreateMap<SearchAudioModel, SearchAudio>()
            .ForMember(d => d.Details, o => o.MapFrom(s => s.Data.Result))
            .ForMember(d => d.Total, o => o.MapFrom(s => s.Data.NumResults))
            .AfterMap((s, d) => d.Details = [.. d.Details.OrderByDescending(x => x.Rank)]);
    }

    [GeneratedRegex("<.*?>")]
    private static partial Regex RemoveHtmlTags();

    private static TimeSpan ParseTimeSpan(string val)
    {
        if (!string.IsNullOrEmpty(val))
        {
            var slices = val
            .Split(':')
            .Select(int.Parse)
            .ToArray();

            return slices.Length == 3
                ? new TimeSpan(slices[0], slices[1], slices[2])
                : slices.Length == 2
                    ? new TimeSpan(0, slices[0], slices[1])
                    : new TimeSpan(0, 0, slices[0]);
        }

        return TimeSpan.Zero;
    }
}