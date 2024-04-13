using Rayer.SearchEngine.Models.Response.Login;

namespace Rayer.SearchEngine.Business.Login.Abstractions;

public interface IAnonymousService
{
    Task<AnonymousResponse> AnonymousAsync();
}