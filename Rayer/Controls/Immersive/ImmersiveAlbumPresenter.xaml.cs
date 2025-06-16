using Rayer.ViewModels;
using System.Windows.Controls;

namespace Rayer.Controls.Immersive;

public partial class ImmersiveAlbumPresenter : UserControl
{
    public ImmersiveAlbumPresenter()
    {
        var vm = App.GetRequiredService<ImmersiveAlbumPresenterViewModel>();

        ViewModel = vm;
        DataContext = this;

        InitializeComponent();
    }

    public ImmersiveAlbumPresenterViewModel ViewModel { get; set; }
}