using Rayer.Command.Parameter;
using Rayer.Core.Abstractions;
using Rayer.Core.Common;
using Rayer.Core.Events;
using Rayer.Core.Utils;
using Rayer.ViewModels;
using System.Collections.Specialized;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
using Wpf.Ui.Appearance;

namespace Rayer.Controls;

public partial class RightPlaybarPanel : UserControl
{
    private readonly IAudioManager _audioManager;

    public RightPlaybarPanel()
    {
        var vm = App.GetRequiredService<RightPlaybarPanelViewModel>();

        ViewModel = vm;
        DataContext = this;

        _audioManager = App.GetRequiredService<IAudioManager>();

        _audioManager.AudioChanged += AudioChanged;
        _audioManager.Audios.CollectionChanged += OnAudiosCollectionChanged;
        _audioManager.Playback.Queue.CollectionChanged += OnPlayQueueCollectionChanged;

        ViewModel.Items.AddRange(_audioManager.Playback.Queue);

        ViewModel.QueueCount = $"共{ViewModel.Items.Count}首歌曲";

        ApplicationThemeManager.Changed += ThemeChanged;

        InitializeComponent();
    }

    public RightPlaybarPanelViewModel ViewModel { get; set; }

    private void OnMouseUp(object sender, MouseButtonEventArgs e)
    {
        ViewModel.OnButtonClick();

        LibListView.ScrollIntoView(_audioManager.Playback.Audio);
    }

    private void OnRecycleMouseUp(object sender, MouseButtonEventArgs e)
    {
        ViewModel.Items.Clear();
        ViewModel.QueueCount = $"共0首歌曲";

        if (_audioManager.Playback.Playing)
        {
            _audioManager.Playback.EndPlay();
        }

        _audioManager.Playback.Queue.Clear();
    }

    private void AudioChanged(object? sender, AudioChangedArgs e)
    {
        var index = ViewModel.Items.IndexOf(e.New);
        LibListView.SelectedIndex = index;
        LibListView.ScrollIntoView(e.New);
    }

    private void OnAudiosCollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
    {
        Application.Current.Dispatcher.Invoke(() =>
        {
            if (e.Action is NotifyCollectionChangedAction.Add)
            {
                var startIndex = e.NewStartingIndex;
                if (e.NewItems is not null)
                {
                    foreach (var item in e.NewItems)
                    {
                        ViewModel.Items.Insert(startIndex++, (Audio)item);
                    }
                }
            }
            if (e.Action is NotifyCollectionChangedAction.Remove)
            {
                if (ViewModel.Items.Count > e.OldStartingIndex)
                {
                    ViewModel.Items.RemoveAt(e.OldStartingIndex);
                }
            }

            ViewModel.QueueCount = $"共{ViewModel.Items.Count}首歌曲";
        });
    }

    private void OnPlayQueueCollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
    {
        Application.Current.Dispatcher.Invoke(() =>
        {
            if (e.Action is NotifyCollectionChangedAction.Add)
            {
                var startIndex = e.NewStartingIndex;
                if (e.NewItems is not null)
                {
                    foreach (var item in e.NewItems)
                    {
                        ViewModel.Items.Insert(startIndex++, (Audio)item);
                    }
                }
            }
            if (e.Action is NotifyCollectionChangedAction.Remove)
            {
                ViewModel.Items.RemoveAt(e.OldStartingIndex);
            }
            else if (e.Action is NotifyCollectionChangedAction.Reset)
            {
                ViewModel.Items.Clear();
            }

            ViewModel.QueueCount = $"共{ViewModel.Items.Count}首歌曲";
        });
    }

    private async void OnListViewItemDoubleClick(object sender, MouseButtonEventArgs e)
    {
        if (e.Source is ListViewItem listViewItem &&
            listViewItem.DataContext is Audio item)
        {
            await ViewModel.Play(item);
        }
    }

    private void ThemeChanged(ApplicationTheme currentApplicationTheme, Color systemAccent)
    {
        PlayQueue.Source = (ImageSource)Application.Current.Resources["PlayQueue"];
        Recycle.Source = (ImageSource)Application.Current.Resources["Recycle"];
    }

    private void OnListViewItemRightButtonUp(object sender, MouseButtonEventArgs e)
    {
        foreach (var item in ViewModel.ContextMenu.Items.SourceCollection)
        {
            if (item is MenuItem menuItem)
            {
                if (sender is FrameworkElement { DataContext: Audio audio })
                {
                    menuItem.CommandParameter = new AudioCommandParameter()
                    {
                        Audio = audio,
                        Scope = ContextMenuScope.PlayQueue
                    };
                }

                if (menuItem.Header is string header)
                {
                    if (header == "播放")
                    {
                        menuItem.Icon = ImageIconFactory.Create("Play", 18);
                    }
                    else if (header == "添加到")
                    {
                        menuItem.Icon = ImageIconFactory.Create("AddTo", 18);
                    }
                    else if (header == "删除")
                    {
                        menuItem.Icon = ImageIconFactory.Create("Recycle", 18);
                    }
                }
            }
        }
    }

    private void OnLoaded(object sender, RoutedEventArgs e)
    {
        var popup = (Popup)Flyout.Template.FindName("PART_Popup", Flyout);

        popup.CustomPopupPlacementCallback = new CustomPopupPlacementCallback((popupSize, targetSize, offset) =>
        {
            var wnd = App.MainWindow;

            var screen = System.Windows.Forms.Screen.FromHandle(new WindowInteropHelper(wnd).Handle);

            var distanceToRight = 0d;

            if (screen is not null)
            {
                distanceToRight = screen.WorkingArea.Right - (wnd.Left + wnd.ActualWidth);
            }

            return distanceToRight > popupSize.Width - targetSize.Width - 42
                ? [new(
                    new Point(
                        -targetSize.Width + 10,
                        (popupSize.Height * -1) - targetSize.Height),
                    PopupPrimaryAxis.Vertical)]
                : [new(
                    new Point(
                        -popupSize.Width + 38,
                        (popupSize.Height * -1) - targetSize.Height),
                    PopupPrimaryAxis.Vertical)];

        });
    }
}