using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Wpf.Ui;

namespace Rayer.SearchEngine.ViewModels.Explore;

public partial class ExploreViewModelBase(INavigationService navigationService) : ObservableObject
{
    public object? DataContext { get; set; }

    [RelayCommand]
    public void NavigateForward(Type type)
    {
        _ = navigationService.NavigateWithHierarchy(type, DataContext);
    }

    [RelayCommand]
    public void NavigateBack()
    {
        _ = navigationService.GoBack();
    }
}