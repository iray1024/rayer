using Rayer.SearchEngine.Abstractions;
using Rayer.SearchEngine.Internal.Abstractions;

namespace Rayer.SearchEngine.Services;

internal class SearchEngine : SearchEngineBase, ISearchEngine
{

    public SearchEngine(IServiceProvider serviceProvider) : base(serviceProvider)
    {

    }
}