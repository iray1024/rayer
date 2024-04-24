using Rayer.SearchEngine.Core.Domain.Playlist;
using Rayer.SearchEngine.Core.Enums;
using System.Windows;
using System.Windows.Controls;

namespace Rayer.SearchEngine.Controls.TemplateSelector;

public class ExploreAlbumTemplateSelector : DataTemplateSelector
{
    public DataTemplate AlbumTemplate { get; set; } = null!;
    public DataTemplate AudioTemplate { get; set; } = null!;
    public DataTemplate PlaylistTemplate { get; set; } = null!;

    public override DataTemplate SelectTemplate(object item, DependencyObject container)
    {
        return item is PlaylistDetail detail
            ? detail.Type switch
            {
                SearchType.Audio => AudioTemplate,
                SearchType.Artist => throw new NotImplementedException(),
                SearchType.Album => AlbumTemplate,
                SearchType.Video => throw new NotImplementedException(),
                SearchType.Playlist => PlaylistTemplate,
                _ => throw new NotImplementedException(),
            }
            : base.SelectTemplate(item, container);
    }
}