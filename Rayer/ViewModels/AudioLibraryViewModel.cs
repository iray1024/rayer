using CommunityToolkit.Mvvm.ComponentModel;
using Rayer.Core.Models;
using System.ComponentModel;
using System.Windows.Data;

namespace Rayer.ViewModels;

public partial class AudioLibraryViewModel : ObservableObject
{
    [ObservableProperty]
    private CollectionViewSource _items = default!;

    public AudioLibraryViewModel()
    {
        Items = new CollectionViewSource();
        Items.SortDescriptions.Add(new SortDescription(nameof(Audio.Title), ListSortDirection.Ascending));
    }
}