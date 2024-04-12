using Microsoft.Extensions.DependencyInjection;
using Rayer.SearchEngine.Internal.ApiSelector;

namespace Rayer.SearchEngine.Internal.Abstractions;

internal abstract class SearchEngineBase
{
    protected SearchEngineBase(IServiceProvider serviceProvider)
    {
        ServiceProvider = serviceProvider;

        Search = serviceProvider.GetRequiredService<IRequestService>();
        Login = serviceProvider.GetRequiredService<LoginApiSelector>();
        Account = serviceProvider.GetRequiredService<AccountApiSelector>();
    }

    protected IServiceProvider ServiceProvider { get; }

    protected IRequestService Search { get; }

    protected LoginApiSelector Login { get; }
    protected AccountApiSelector Account { get; }
}