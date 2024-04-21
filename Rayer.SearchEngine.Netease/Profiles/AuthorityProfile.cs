using AutoMapper;
using Rayer.SearchEngine.Core.Enums;
using Rayer.SearchEngine.Netease.Models.Login.Authority;

namespace Rayer.SearchEngine.Netease.Profiles;

internal class AuthorityProfile : Profile
{
    public AuthorityProfile()
    {
        CreateMap<AccountModel, Core.Domain.Authority.Account>()
            .ForMember(d => d.Anonimous, o => o.MapFrom(s => s.AnonimousUser));

        CreateMap<ProfileModel, Core.Domain.Authority.Profile>()
            .ForMember(d => d.Name, o => o.MapFrom(s => s.NickName))
            .ForMember(d => d.Birthday, o => o.MapFrom(s =>
                DateOnly.FromDateTime(DateTimeOffset.FromUnixTimeMilliseconds(s.Birthday).LocalDateTime)))
            .ForMember(d => d.Gender, o => o.MapFrom(s =>
                s.Gender == 1 ? Gender.Male : s.Gender == 2 ? Gender.Female : Gender.Unknown));

        CreateMap<ProfileModel, Core.Domain.Authority.User>()
            .ForPath(d => d.Account.Id, o => o.MapFrom(s => s.UserId))
            .ForMember(d => d.Profile, o => o.MapFrom((s, _, _, ctx) => ctx.Mapper.Map<Core.Domain.Authority.Profile>(s)));

        CreateMap<UserModel, Core.Domain.Authority.User>();
    }
}