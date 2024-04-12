using Rayer.SearchEngine.Models.Response;

namespace Rayer.SearchEngine.Login.Abstractions;

public interface IAnonymousService
{
    Task<AnonymousResponse> AnonymousAsync();
}