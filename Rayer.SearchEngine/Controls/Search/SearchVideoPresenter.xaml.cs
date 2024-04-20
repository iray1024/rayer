using Rayer.Core;
using Rayer.SearchEngine.Models.Response.Netease.Search;
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

public partial class SearchVideoPresenter : UserControl, IPresenterControl<SearchVideoPresenterViewModel, SearchVideoDetail>
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
