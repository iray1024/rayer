using Rayer.SearchEngine.Models.Response.Login;

namespace Rayer.SearchEngine.Login.Abstractions;

public interface IAnonymousService
{
    Task<AnonymousResponse> AnonymousAsync();
}