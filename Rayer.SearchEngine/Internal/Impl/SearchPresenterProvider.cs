using Rayer.Core;
using Rayer.Core.Framework.Injection;
using Rayer.SearchEngine.Controls;
using Rayer.SearchEngine.Controls.Search;
using Rayer.SearchEngine.Core.Enums;
using Rayer.SearchEngine.Internal.Abstractions;
using Rayer.SearchEngine.ViewModels.Presenter;
using System.Windows;

namespace Rayer.SearchEngine.Internal.Impl;

[Inject<ISearchPresenterProvider>]
internal class SearchPresenterProvider : ISearchPresenterProvider
{
    private static readonly Lazy<SearchAudioPresenter> _audioPresenter;
    private static readonly Lazy<SearchArtistPresenter> _artistPresenter;
    private static readonly Lazy<SearchAlbumPresenter> _albumPresenter;
    private static readonly Lazy<SearchVideoPresenter> _videoPresenter;
    private static readonly Lazy<SearchPlaylistPresenter> _plyalistPresenter;

    static SearchPresenterProvider()
    {
        _audioPresenter = new Lazy<SearchAudioPresenter>(() => new SearchAudioPresenter(AppCore.GetRequiredService<SearchAudioPresenterViewModel>())
        {
            HorizontalAlignment = HorizontalAlignment.Left,
            VerticalAlignment = VerticalAlignment.Top,
        });

        _artistPresenter = new Lazy<SearchArtistPresenter>(() => new SearchArtistPresenter()
        {
            HorizontalAlignment = HorizontalAlignment.Left,
            VerticalAlignment = VerticalAlignment.Top,
        });

        _albumPresenter = new Lazy<SearchAlbumPresenter>(() => new SearchAlbumPresenter()
        {
            HorizontalAlignment = HorizontalAlignment.Left,
            VerticalAlignment = VerticalAlignment.Top,
        });

        _videoPresenter = new Lazy<SearchVideoPresenter>(() => new SearchVideoPresenter()
        {
            HorizontalAlignment = HorizontalAlignment.Left,
            VerticalAlignment = VerticalAlignment.Top,
        });

        _plyalistPresenter = new Lazy<SearchPlaylistPresenter>(() => new SearchPlaylistPresenter()
        {
            HorizontalAlignment = HorizontalAlignment.Left,
            VerticalAlignment = VerticalAlignment.Top,
        });
    }

    public IPresenterControl<TViewModel, TResponse> GetPresenter<TViewModel, TResponse>(SearchType searchType)
        where TViewModel : IPresenterViewModel<TResponse>
        where TResponse : class
    {
        return searchType switch
        {
            SearchType.Audio => (IPresenterControl<TViewModel, TResponse>)_audioPresenter.Value,

            SearchType.Artist => (IPresenterControl<TViewModel, TResponse>)_artistPresenter.Value,

            SearchType.Album => (IPresenterControl<TViewModel, TResponse>)_albumPresenter.Value,

            SearchType.Video => (IPresenterControl<TViewModel, TResponse>)_videoPresenter.Value,

            SearchType.Playlist => (IPresenterControl<TViewModel, TResponse>)_plyalistPresenter.Value,
            _ => throw new NotImplementedException()
        };
    }
}