using Rayer.FrameworkCore;
using Rayer.FrameworkCore.Injection;
using Rayer.SearchEngine.ViewModels.Explore;
using Wpf.Ui.Abstractions.Controls;

namespace Rayer.SearchEngine.Views.Pages.Explore;

[Inject]
public partial class ExploreSpotPage : INavigableView<ExploreSpotViewModel>
{
    public ExploreSpotPage()
    {
        var vm = AppCore.GetRequiredService<ExploreSpotViewModel>();

        ViewModel = vm;
        DataContext = this;

        InitializeComponent();
    }

    public ExploreSpotViewModel ViewModel { get; set; }
}