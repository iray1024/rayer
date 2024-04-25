using System.Windows.Controls;
using System.Windows.Media;
using Wpf.Ui.Appearance;

namespace Rayer.SearchEngine.Controls;

public partial class Loader : UserControl
{
    public Loader()
    {
        InitializeComponent();

        ApplicationThemeManager.Changed += OnThemeChanged;
    }

    private void OnThemeChanged(ApplicationTheme currentApplicationTheme, Color systemAccent)
    {
        PART_Loader.Foreground = currentApplicationTheme switch
        {
            ApplicationTheme.Light => new SolidColorBrush(Color.FromRgb(44, 44, 44)),
            ApplicationTheme.Dark => new SolidColorBrush(Colors.LightGray),
            _ => new SolidColorBrush(Colors.LightSlateGray)
        };
    }
}