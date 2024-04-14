namespace Rayer.SearchEngine.Controls;

internal interface IPresenterViewModel<T>
    where T : class
{
    T PresenterDataContext { get; set; }
}