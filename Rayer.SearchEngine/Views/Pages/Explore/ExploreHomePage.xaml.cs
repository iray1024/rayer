using Rayer.FrameworkCore;
using Rayer.FrameworkCore.Injection;
using Rayer.SearchEngine.ViewModels.Explore;
using Wpf.Ui.Abstractions.Controls;

namespace Rayer.SearchEngine.Views.Pages.Explore;

[Inject]
public partial class ExploreHomePage : INavigableView<ExploreHomeViewModel>
{
    public ExploreHomePage()
    {
        var vm = AppCore.GetRequiredService<ExploreHomeViewModel>();

        ViewModel = vm;
        DataContext = this;

        InitializeComponent();
    }

    public ExploreHomeViewModel ViewModel { get; set; }
}