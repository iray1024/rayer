using Microsoft.Extensions.DependencyInjection;
using Rayer.Installer.ViewModels;

namespace Rayer.Installer.Transitions;

public partial class Slide4 : System.Windows.Controls.UserControl
{
    public Slide4()
    {
        InitializeComponent();

        DataContext = App.Services.GetRequiredService<MainViewModel>();
    }
}