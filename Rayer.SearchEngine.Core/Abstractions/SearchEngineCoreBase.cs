using AutoMapper;
using Microsoft.Extensions.DependencyInjection;
using Rayer.Core.Common;
using Rayer.FrameworkCore;
using Rayer.SearchEngine.Core.Http.Abstractions;

namespace Rayer.SearchEngine.Core.Abstractions;

public abstract class SearchEngineCoreBase
{
    protected SearchEngineCoreBase(SearcherType searcherType = SearcherType.Netease)
    {
        ServiceProvider = AppCore.ServiceProvider;

        Searcher = ServiceProvider.GetRequiredKeyedService<IRequestService>(searcherType);
        Mapper = ServiceProvider.GetRequiredService<IMapper>();
    }

    protected IServiceProvider ServiceProvider { get; }

    protected IRequestService Searcher { get; }

    protected IMapper Mapper { get; }
}