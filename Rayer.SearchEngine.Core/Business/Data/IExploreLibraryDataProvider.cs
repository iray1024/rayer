using Rayer.SearchEngine.Core.Domain.Aggregation;

namespace Rayer.SearchEngine.Core.Business.Data;

public interface IExploreLibraryDataProvider
{
    public ExploreLibraryModel Model { get; set; }

    event EventHandler Loaded;
}