namespace Rayer.SearchEngine.Controls;

internal interface IPresenterControl<TViewModel, TResponse>
    where TViewModel : IPresenterViewModel<TResponse>
    where TResponse : class
{
    TViewModel ViewModel { get; set; }
}