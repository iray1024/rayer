using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using Wpf.Ui.Appearance;
using Wpf.Ui.Controls;

namespace Rayer.Controls;

public partial class KeybindingPanel : UserControl
{
    public KeybindingPanel()
    {
        InitializeComponent();

        ApplicationThemeManager.Changed += ThemeChanged;
    }

    private void ThemeChanged(ApplicationTheme currentApplicationTheme, Color systemAccent)
    {
        PitchUp.Icon = new ImageIcon
        {
            Width = 24,
            Height = 24,
            Source = (ImageSource)Application.Current.Resources["Pitch"]
        };

        PitchDown.Icon = new ImageIcon
        {
            Width = 24,
            Height = 24,
            Source = (ImageSource)Application.Current.Resources["Pitch"]
        };
    }
}