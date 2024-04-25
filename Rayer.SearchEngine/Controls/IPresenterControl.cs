namespace Rayer.SearchEngine.Controls;

internal interface IPresenterControl<TViewModel, TContext>
    where TViewModel : IPresenterViewModel<TContext>
    where TContext : class
{
    TViewModel ViewModel { get; set; }
}