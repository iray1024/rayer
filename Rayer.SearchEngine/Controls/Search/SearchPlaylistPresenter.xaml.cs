using Rayer.Core;
using Rayer.SearchEngine.Core.Domain.Playlist;
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

public partial class SearchPlaylistPresenter : UserControl, IPresenterControl<SearchPlaylistPresenterViewModel, SearchPlaylist>
{
    public SearchPlaylistPresenter()
    {
        var vm = AppCore.GetRequiredService<SearchPlaylistPresenterViewModel>();

        ViewModel = vm;
        DataContext = this;

        InitializeComponent();
    }

    public SearchPlaylistPresenterViewModel ViewModel { get; set; }
}