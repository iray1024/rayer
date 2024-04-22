using Rayer.Core;
using Rayer.SearchEngine.Core.Domain.Video;
using Rayer.SearchEngine.ViewModels.Presenter;
using System.Windows.Controls;

namespace Rayer.SearchEngine.Controls.Search;

public partial class SearchVideoPresenter : UserControl, IPresenterControl<SearchVideoPresenterViewModel, SearchVideo>
{
    public SearchVideoPresenter()
    {
        var vm = AppCore.GetRequiredService<SearchVideoPresenterViewModel>();

        ViewModel = vm;
        DataContext = this;

        InitializeComponent();
    }

    public SearchVideoPresenterViewModel ViewModel { get; set; }
}