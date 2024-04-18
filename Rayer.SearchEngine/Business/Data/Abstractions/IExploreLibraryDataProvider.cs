using Rayer.SearchEngine.Models.Domian;

namespace Rayer.SearchEngine.Business.Data.Abstractions;

public interface IExploreLibraryDataProvider
{
    public ExploreLibraryModel Model { get; set; }

    event EventHandler Loaded;
}