using CommunityToolkit.Mvvm.ComponentModel;
using Rayer.Core.Abstractions;
using Rayer.Core.Models;
using System.Collections.ObjectModel;

namespace Rayer.ViewModels;

public partial class AudioLibraryViewModel : ObservableObject
{
    [ObservableProperty]
    private ObservableCollection<Audio> _items = [];

    public AudioLibraryViewModel(IAudioManager audioManager)
    {

    }

    partial void OnItemsChanged(ObservableCollection<Audio>? oldValue, ObservableCollection<Audio> newValue)
    {

    }
}