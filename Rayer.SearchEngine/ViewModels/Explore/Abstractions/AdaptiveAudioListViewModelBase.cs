using CommunityToolkit.Mvvm.ComponentModel;
using System.Windows;

namespace Rayer.SearchEngine.ViewModels.Explore.Abstractions;

// 所有需要用List来展示Audio的控件的ViewModel都可以使用该类继承，以提供内容自适应的能力
public partial class AdaptiveAudioListViewModelBase : ObservableObject
{
    [ObservableProperty]
    private double _nameMaxWidth = 150;

    [ObservableProperty]
    private double _artistsNameMaxWidth = 250;

    [ObservableProperty]
    private double _albumNameMaxHeight = 250;

    [ObservableProperty]
    private double _durationMaxHeight = 35;

    [ObservableProperty]
    private Thickness _itemMargin = new(0, 0, 28, 0);
}