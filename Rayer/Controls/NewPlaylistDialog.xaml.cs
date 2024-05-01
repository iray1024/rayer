using System.Windows.Controls;
using Wpf.Ui.Controls;

namespace Rayer.Controls;

public partial class NewPlaylistDialog : ContentDialog
{
    public NewPlaylistDialog(ContentPresenter? contentPresenter)
        : base(contentPresenter)
    {
        InitializeComponent();
    }
}