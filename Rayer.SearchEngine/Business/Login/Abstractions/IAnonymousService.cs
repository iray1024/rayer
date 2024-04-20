using Rayer.SearchEngine.Models.Response.Netease.Login;

namespace Rayer.SearchEngine.Business.Login.Abstractions;

public interface IAnonymousService
{
    Task<AnonymousUser> AnonymousAsync();
}