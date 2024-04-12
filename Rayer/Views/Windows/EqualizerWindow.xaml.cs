using Rayer.Core.Framework.Settings.Abstractions;
using Rayer.Core.Utils;
using Rayer.ViewModels;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using Wpf.Ui.Appearance;

namespace Rayer.Views.Windows;

public partial class EqualizerWindow
{
    public EqualizerWindow()
    {
        SystemThemeWatcher.Watch(this);

        var vm = App.GetRequiredService<EqualizerViewModel>();

        ViewModel = vm;
        DataContext = this;

        InitializeComponent();

        Initialize();
    }

    public EqualizerViewModel ViewModel { get; set; }

    private void Initialize()
    {
        var mode = App.GetRequiredService<ISettingsService>().Settings.EqualizerMode;

        var identifier = EnumHelper.GetDescription(mode);

        foreach (var item in EqualizerModeGroup.Children)
        {
            if (item is RadioButton radio && radio.Content is string _id && _id == identifier)
            {
                radio.IsChecked = true;
            }
        }
    }

    #region Slider Events
    private void OnDragStarted(object sender, DragStartedEventArgs e)
    {
        CustomRadio.IsChecked = true;

        ViewModel.SwitchToCustom();
    }

    private void OnDragCompleted(object sender, DragCompletedEventArgs e)
    {
        ViewModel.SaveCustom();
    }

    private void OnPreviewMouseDown(object sender, MouseButtonEventArgs e)
    {
        CustomRadio.IsChecked = true;

        ViewModel.SwitchToCustom();
        ViewModel.SaveCustom();
    }

    private void OnMouseWheel(object sender, MouseWheelEventArgs e)
    {
        CustomRadio.IsChecked = true;

        ViewModel.SwitchToCustom();

        if (sender is Slider slider)
        {
            var value = slider.Value + (0.5 * (e.Delta > 0 ? 1 : -1));

            value = Math.Min(Math.Max(value, -12f), 12f);

            slider.Value = value;
        }

        ViewModel.SaveCustom();
    }
    #endregion

    private void OnMouseEnter(object sender, MouseEventArgs e)
    {
        if (sender is Slider slider && slider.Parent is StackPanel panel)
        {
            var textBlock = panel.Children[0];

            if (textBlock is TextBlock block)
            {
                block.Visibility = Visibility.Visible;
            }
        }
    }

    private void OnMouseLeave(object sender, MouseEventArgs e)
    {
        if (sender is Slider slider && slider.Parent is StackPanel panel)
        {
            var textBlock = panel.Children[0];

            if (textBlock is TextBlock block)
            {
                block.Visibility = Visibility.Hidden;
            }
        }
    }
}