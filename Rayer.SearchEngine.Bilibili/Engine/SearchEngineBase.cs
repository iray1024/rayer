using Rayer.Core.Common;
using Rayer.SearchEngine.Core.Abstractions;

namespace Rayer.SearchEngine.Bilibili.Engine;

internal abstract class SearchEngineBase : SearchEngineCoreBase
{
    protected SearchEngineBase()
        : base(SearcherType.Bilibili)
    {

    }
}