using Rayer.Core.Framework.Injection;
using Rayer.SearchEngine.Controls;
using Rayer.SearchEngine.Controls.Search;
using Rayer.SearchEngine.Internal.Abstractions;
using System.Windows;

namespace Rayer.SearchEngine.Internal.Impl;

[Inject<ISearchPresenterProvider>]
internal class SearchPresenterProvider : ISearchPresenterProvider
{
    private static readonly Lazy<SearchAudioPresenter> _audioPresenter;

    static SearchPresenterProvider()
    {
        _audioPresenter = new Lazy<SearchAudioPresenter>(() => new SearchAudioPresenter()
        {
            HorizontalAlignment = HorizontalAlignment.Left,
            VerticalAlignment = VerticalAlignment.Top,
        });
    }

    public IPresenterControl<TViewModel, TResponse> GetPresenter<TViewModel, TResponse>(string identifier)
        where TViewModel : IPresenterViewModel<TResponse>
        where TResponse : class
    {
        if (identifier is "歌曲")
        {
            return (IPresenterControl<TViewModel, TResponse>)_audioPresenter.Value;
        }
        else if (identifier is "艺人")
        {

        }
        else if (identifier is "专辑")
        {

        }
        else if (identifier is "视频")
        {

        }
        else if (identifier is "歌单")
        {

        }

        return null!;
    }
}