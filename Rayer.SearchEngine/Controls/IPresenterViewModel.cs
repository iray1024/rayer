namespace Rayer.SearchEngine.Controls;

public interface IPresenterViewModel
{

}

public interface IPresenterViewModel<T> : IPresenterViewModel
    where T : class
{
    T PresenterDataContext { get; set; }

    void ResetData()
    {
        PresenterDataContext = null!;
    }
}