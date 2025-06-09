using Rayer.FrameworkCore.Injection;
using Rayer.ViewModels;
using System.Windows.Controls;

namespace Rayer.Controls;

[Inject]
public partial class PlaylistTitleBar : UserControl
{
    public PlaylistTitleBar(PlaylistPageViewModel viewModel)
    {
        ViewModel = viewModel;
        DataContext = this;

        InitializeComponent();
    }

    public PlaylistPageViewModel ViewModel { get; set; }
}