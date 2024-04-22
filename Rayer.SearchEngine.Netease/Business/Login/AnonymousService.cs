using Microsoft.Extensions.DependencyInjection;
using Rayer.Core.Common;
using Rayer.Core.Framework.Injection;
using Rayer.SearchEngine.Core.Business.Login;
using Rayer.SearchEngine.Core.Domain.Authority;
using Rayer.SearchEngine.Netease.Engine;
using Rayer.SearchEngine.Netease.Models.Login;

namespace Rayer.SearchEngine.Business.Login.Impl;

[Inject<IAnonymousService>(ServiceLifetime = ServiceLifetime.Scoped, ServiceKey = SearcherType.Netease)]
internal class AnonymousService : SearchEngineBase, IAnonymousService
{
    public async Task<AnonymousUser> AnonymousAsync()
    {
        var anonymousResult = await Searcher.GetAsync(
            LoginSelector.AnonymousLogin()
                .Build());

        var response = anonymousResult.ToEntity<AnonymousUserModel>();

        if (response is not null)
        {
            var domain = Mapper.Map<AnonymousUser>(response);

            return domain;
        }

        return default!;
    }
}