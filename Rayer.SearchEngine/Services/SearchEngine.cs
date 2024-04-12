using Rayer.SearchEngine.Abstractions;
using Rayer.SearchEngine.Internal.Abstractions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rayer.SearchEngine.Services;

internal class SearchEngine : SearchEngineBase, ISearchEngine
{

    public SearchEngine(IServiceProvider serviceProvider) : base(serviceProvider)
    {

    }


}