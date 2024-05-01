using Rayer.Core;
using Rayer.Core.Abstractions;
using Rayer.Core.Controls;
using Rayer.Core.Events;
using Rayer.Core.Models;
using Rayer.Core.Utils;
using Rayer.SearchEngine.Core.Domain.Aduio;
using Rayer.SearchEngine.ViewModels.Presenter;
using System.Windows;
using System.Windows.Controls;
using Wpf.Ui;
using Wpf.Ui.Controls;
using ListViewItem = Rayer.Core.Controls.ListViewItem;

namespace Rayer.SearchEngine.Controls.Search;

public partial class SearchAudioPresenter : AdaptiveUserControl, IPresenterControl<SearchAudioPresenterViewModel, SearchAudio>
{
    private bool _isLoaded = false;

    public SearchAudioPresenter(
        SearchAudioPresenterViewModel vm)
        : base(vm)
    {
        ViewModel = vm;
        DataContext = this;

        ViewModel.DataChanged += OnDataChanged;

        var audioManager = AppCore.GetRequiredService<IAudioManager>();

        audioManager.AudioChanged += OnAudioChanged;
        audioManager.AudioStopped += OnAudioStopped;

        InitializeComponent();
    }

    public new SearchAudioPresenterViewModel ViewModel { get; set; }

    protected override void OnLoaded(object sender, RoutedEventArgs e)
    {
        if (!_isLoaded)
        {
            ((Grid)Parent).SizeChanged += OnParentSizeChanged;
            _isLoaded = true;
        }

        var audioManager = AppCore.GetRequiredService<IAudioManager>();

        if (audioManager.Playback.Audio is Audio audio && audioManager.Playback.Playing)
        {
            var navView = AppCore.GetRequiredService<INavigationService>().GetNavigationControl() as NavigationView;

            if (navView?.Template.FindName("PART_NavigationViewContentPresenter", navView) is NavigationViewContentPresenter navPresenter)
            {
                var scrollViewer = ElementHelper.GetScrollViewer(LibListView);
                var exScrollViewer = ElementHelper.GetScrollViewer(navPresenter);

                scrollViewer?.ScrollToTop();

                var index = -1;
                for (var i = 0; i < LibListView.Items.Count; i++)
                {
                    if (LibListView.Items[i] is SearchAudioDetail vDetail && vDetail.Id == audio.Id)
                    {
                        index = i;

                        break;
                    }
                }

                if (index != -1)
                {
                    scrollViewer?.ScrollToVerticalOffset(56 * index);
                    exScrollViewer?.ScrollToVerticalOffset(56 * index);
                }
            }
        }
    }

    private void OnAudioChanged(object? sender, AudioChangedArgs e)
    {
        foreach (var listviewItem in LibListView.Items)
        {
            var vContainer = LibListView.ItemContainerGenerator.ContainerFromItem(listviewItem);

            if (vContainer is ListViewItem vItem)
            {
                if (vItem.DataContext is SearchAudioDetail detail)
                {
                    if (detail.Id == e.New.Id)
                    {
                        var index = LibListView.Items.IndexOf(vItem.DataContext);
                        LibListView.SelectedIndex = index;
                        LibListView.ScrollIntoView(vItem.DataContext);
                        vItem.IsSelected = true;
                    }
                    else
                    {
                        vItem.IsSelected = false;
                    }
                }
            }
        }
    }

    private void OnAudioStopped(object? sender, EventArgs e)
    {
        LibListView.SelectedIndex = -1;
        foreach (var listviewItem in LibListView.Items)
        {
            var vContainer = LibListView.ItemContainerGenerator.ContainerFromItem(listviewItem);

            if (vContainer is ListViewItem vItem)
            {
                vItem.IsSelected = false;
            }
        }
    }

    private void OnListViewItemRightButtonUp(object sender, System.Windows.Input.MouseButtonEventArgs e)
    {

    }

    private async void OnListViewItemDoubleClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
    {
        if (e.Source is ListViewItem listViewItem &&
            listViewItem.DataContext is SearchAudioDetail item)
        {
            if (item.Copyright.HasCopyright)
            {
                foreach (var listviewItem in LibListView.Items)
                {
                    var vContainer = LibListView.ItemContainerGenerator.ContainerFromItem(listviewItem);

                    if (vContainer is ListViewItem vItem)
                    {
                        vItem.IsSelected = false;
                    }
                }

                listViewItem.IsSelected = true;

                await ViewModel.PlayWebAudio(item);
            }
        }
    }

    private void OnDataChanged(object? sender, EventArgs e)
    {
        AppCore.MainWindow.Width += 1;
        AppCore.MainWindow.Width -= 1;
    }

    private void OnParentSizeChanged(object sender, SizeChangedEventArgs e)
    {
        Width = e.NewSize.Width;

        Resize(AppCore.MainWindow.ActualWidth, e);
    }
}