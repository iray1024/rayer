using Rayer.ViewModels;
using System.Windows.Controls;
using System.Windows.Input;

namespace Rayer.Controls;

public partial class PitchPanel : UserControl
{
    public PitchPanel()
    {
        var vm = App.GetRequiredService<PitchPanelViewModel>();

        ViewModel = vm;
        DataContext = this;

        InitializeComponent();
    }

    public PitchPanelViewModel ViewModel { get; set; }

    private void OnMouseUp(object sender, MouseButtonEventArgs e)
    {
        ViewModel.Reset();
    }

    private void OnMouseEnter(object sender, MouseEventArgs e)
    {
        PitchValue.Visibility = System.Windows.Visibility.Visible;
    }

    private void OnMouseLeave(object sender, MouseEventArgs e)
    {
        PitchValue.Visibility = System.Windows.Visibility.Hidden;
    }
}