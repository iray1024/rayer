using Rayer.Core;
using Rayer.SearchEngine.Models.Response.Search;
using Rayer.SearchEngine.ViewModels.Presenter;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Rayer.SearchEngine.Controls.Search;

public partial class SearchAlbumPresenter : UserControl, IPresenterControl<SearchAlbumPresenterViewModel, SearchAlbumDetailResponse>
{
    public SearchAlbumPresenter()
    {
        var vm = AppCore.GetRequiredService<SearchAlbumPresenterViewModel>();

        ViewModel = vm;
        DataContext = this;

        InitializeComponent();
    }

    public SearchAlbumPresenterViewModel ViewModel {  get; set; }
}