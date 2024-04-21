using Rayer.SearchEngine.Controls;
using Rayer.SearchEngine.Core.Enums;

namespace Rayer.SearchEngine.Internal.Abstractions;

internal interface ISearchPresenterProvider
{
    IPresenterControl<TViewModel, TResponse> GetPresenter<TViewModel, TResponse>(SearchType searchType)
        where TViewModel : IPresenterViewModel<TResponse>
        where TResponse : class;
}