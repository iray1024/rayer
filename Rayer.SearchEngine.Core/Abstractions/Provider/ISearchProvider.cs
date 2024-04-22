using Rayer.Core.Common;
using System.ComponentModel;

namespace Rayer.SearchEngine.Core.Abstractions.Provider;

[EditorBrowsable(EditorBrowsableState.Never)]
public interface ISearchProvider
{
    SearcherType CurrentSearcher { get; }
}