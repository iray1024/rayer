using Rayer.Core.Framework.Injection;
using Rayer.SearchEngine.Business.Login.Abstractions;
using Rayer.SearchEngine.Extensions;
using Rayer.SearchEngine.Internal.Abstractions;
using Rayer.SearchEngine.Models.Response.Login;

namespace Rayer.SearchEngine.Business.Login.Impl;

[Inject<IAnonymousService>(ServiceLifetime = Microsoft.Extensions.DependencyInjection.ServiceLifetime.Scoped)]
internal class AnonymousService(IServiceProvider serviceProvider) : SearchEngineBase(serviceProvider), IAnonymousService
{
    public async Task<AnonymousResponse> AnonymousAsync()
    {
        var anonymousResult = await Searcher.GetAsync(
            Login.AnonymousLogin()
                .Build());

        var response = anonymousResult.ToEntity<AnonymousResponse>();

        return response is not null ? response : default!;
    }
}