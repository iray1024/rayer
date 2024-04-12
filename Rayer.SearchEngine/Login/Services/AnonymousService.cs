using Rayer.SearchEngine.Extensions;
using Rayer.SearchEngine.Internal.Abstractions;
using Rayer.SearchEngine.Login.Abstractions;
using Rayer.SearchEngine.Models.Response;

namespace Rayer.SearchEngine.Login.Services;

internal class AnonymousService(IServiceProvider serviceProvider) : SearchEngineBase(serviceProvider), IAnonymousService
{
    public async Task<AnonymousResponse> AnonymousAsync()
    {
        var anonymousResult = await Search.GetAsync(
            Login.AnonymousLogin()
                .Build());

        var response = anonymousResult.ToEntity<AnonymousResponse>();

        return response is not null ? response : default!;
    }
}