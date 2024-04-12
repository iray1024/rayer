using Rayer.ViewModels;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using Wpf.Ui.Controls;

namespace Rayer.Controls;

public partial class VolumePanel : UserControl
{
    public VolumePanel()
    {
        var vm = App.GetRequiredService<VolumePanelViewModel>();

        ViewModel = vm;
        DataContext = this;

        InitializeComponent();

        ViewModel.SetDependency(Volume);
    }

    public VolumePanelViewModel ViewModel { get; set; }

    private void OnMouseUp(object sender, MouseButtonEventArgs e)
    {
        ViewModel.OnButtonClick();
    }

    #region Slider Events
    private void Slider_DragDelta(object sender, DragDeltaEventArgs e)
    {
        ViewModel.SetVolume();
    }

    private void Slider_DragCompleted(object sender, DragCompletedEventArgs e)
    {
        ViewModel.SetVolume();

        ViewModel.Save();
    }

    private void Slider_PreviewMouseDown(object sender, MouseButtonEventArgs e)
    {
        var value = 100 - (e.GetPosition(VolumeSlider).Y / VolumeSlider.ActualHeight * (VolumeSlider.Maximum - VolumeSlider.Minimum));

        ViewModel.SetVolume((float)value);

        ViewModel.Save();
    }

    private void OnMouseWheel(object sender, MouseWheelEventArgs e)
    {
        var value = ViewModel.Volume + (e.Delta / 20.0f);

        value = Math.Min(Math.Max(value, 0f), 100);

        ViewModel.SetVolume(value);
        ViewModel.Save();
    }
    #endregion

    private void OnLoaded(object sender, RoutedEventArgs e)
    {
        var popup = (Popup)Flyout.Template.FindName("PART_Popup", Flyout);

        if (popup is not null)
        {
            popup.CustomPopupPlacementCallback = new CustomPopupPlacementCallback((popupSize, targetSize, offset) =>
            {
                return [new(
                        new Point(
                            -targetSize.Width + 8,
                            (popupSize.Height * -1) + 6),
                        PopupPrimaryAxis.Vertical)];

            });
        }
    }
}