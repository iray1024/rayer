using Rayer.SearchEngine.Controls;

namespace Rayer.SearchEngine.Internal.Abstractions;

internal interface ISearchPresenterProvider
{
    IPresenterControl<TViewModel, TResponse> GetPresenter<TViewModel, TResponse>(string identifier)
        where TViewModel : IPresenterViewModel<TResponse>
        where TResponse : class;
}