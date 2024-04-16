using Microsoft.Extensions.DependencyInjection;
using Rayer.Installer.ViewModels;

namespace Rayer.Installer.Transitions;

public partial class Slide3 : System.Windows.Controls.UserControl
{
    public Slide3()
    {
        InitializeComponent();

        DataContext = App.Services.GetRequiredService<MainViewModel>();
    }
}