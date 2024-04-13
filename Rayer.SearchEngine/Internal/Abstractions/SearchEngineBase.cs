using Microsoft.Extensions.DependencyInjection;
using Rayer.SearchEngine.Internal.ApiSelector;

namespace Rayer.SearchEngine.Internal.Abstractions;

internal abstract class SearchEngineBase
{
    protected SearchEngineBase(IServiceProvider serviceProvider)
    {
        ServiceProvider = serviceProvider;

        Searcher = serviceProvider.GetRequiredService<IRequestService>();
        Login = serviceProvider.GetRequiredService<LoginApiSelector>();
        Account = serviceProvider.GetRequiredService<AccountApiSelector>();
        Search = serviceProvider.GetRequiredService<SearchApiSelector>();
        Track = serviceProvider.GetRequiredService<TrackApiSelector>();
    }

    protected IServiceProvider ServiceProvider { get; }

    protected IRequestService Searcher { get; }

    protected LoginApiSelector Login { get; }
    protected AccountApiSelector Account { get; }
    protected SearchApiSelector Search { get; }
    protected TrackApiSelector Track { get; }
}