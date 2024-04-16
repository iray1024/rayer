using Microsoft.Extensions.DependencyInjection;
using Rayer.Installer.ViewModels;

namespace Rayer.Installer.Transitions;

public partial class Slide2 : System.Windows.Controls.UserControl
{
    public Slide2()
    {
        InitializeComponent();

        DataContext = App.Services.GetRequiredService<MainViewModel>();
    }
}