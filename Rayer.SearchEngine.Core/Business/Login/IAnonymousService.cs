using Rayer.SearchEngine.Core.Domain.Authority;

namespace Rayer.SearchEngine.Core.Business.Login;

public interface IAnonymousService
{
    Task<AnonymousUser> AnonymousAsync();
}