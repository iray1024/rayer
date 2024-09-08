using CommunityToolkit.Mvvm.ComponentModel;
using hyjiacan.py4n;
using Rayer.Core.Common;
using Rayer.Core.Controls;
using Rayer.Core.Extensions;
using Rayer.Core.Menu;
using System.ComponentModel;
using System.Windows.Controls;
using System.Windows.Data;

namespace Rayer.ViewModels;

public partial class AudioLibraryViewModel : AdaptiveViewModelBase
{
    [ObservableProperty]
    private string _filterText = string.Empty;

    public AudioLibraryViewModel(IContextMenuFactory contextMenuFactory)
    {
        Audios = new SortableObservableCollection<Audio>([], AudioSortComparer.Ascending);
        AudiosView = CollectionViewSource.GetDefaultView(Audios);
        AudiosView.Filter = OnFilter;

        ContextMenu = contextMenuFactory.CreateContextMenu(ContextMenuScope.Library);
    }

    public ICollectionView AudiosView { get; }

    public SortableObservableCollection<Audio> Audios { get; } = default!;

    public ContextMenu ContextMenu { get; }

    private Audio[] _filterResult = [];

    partial void OnFilterTextChanged(string value)
    {
        _filterResult = FilterByNameMatch(Audios, value);

        AudiosView.Refresh();
    }

    private bool OnFilter(object item)
    {
        return (_filterResult.Length == 0 && string.IsNullOrWhiteSpace(FilterText)) || _filterResult.Contains((Audio)item);
    }

    private static Audio[] FilterByNameMatch(IEnumerable<Audio> source, string keyword)
    {
        if (string.IsNullOrWhiteSpace(keyword))
        {
            return source.ToArray();
        }

        var matchedAudios = new List<Audio>();

        foreach (var audio in source)
        {
            if (audio.Title.Contains(keyword) || audio.Artists.Any(x => x.Contains(keyword)))
            {
                matchedAudios.Add(audio);
            }
            else
            {
                var pinyinItems = Pinyin4Net.GetPinyinArray(audio.Title, PinyinFormat.WITHOUT_TONE);

                foreach (var item in pinyinItems.Combine())
                {
                    if (item.Contains(keyword))
                    {
                        matchedAudios.Add(audio);
                    }
                }

                foreach (var artist in audio.Artists)
                {
                    var artistPinyinItems = Pinyin4Net.GetPinyinArray(artist, PinyinFormat.WITHOUT_TONE);

                    foreach (var item in artistPinyinItems.Combine())
                    {
                        if (item.Contains(keyword))
                        {
                            matchedAudios.Add(audio);
                        }
                    }
                }
            }
        }

        return matchedAudios.DistinctBy(x => x.Id).ToArray();
    }
}