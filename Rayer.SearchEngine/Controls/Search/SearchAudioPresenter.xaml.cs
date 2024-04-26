using Rayer.Core;
using Rayer.Core.Abstractions;
using Rayer.Core.Controls;
using Rayer.Core.Events;
using Rayer.SearchEngine.Core.Domain.Aduio;
using Rayer.SearchEngine.ViewModels.Presenter;
using ListViewItem = Rayer.Core.Controls.ListViewItem;

namespace Rayer.SearchEngine.Controls.Search;

public partial class SearchAudioPresenter : AdaptiveUserControl, IPresenterControl<SearchAudioPresenterViewModel, SearchAudio>
{
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

    private void OnDataChanged(object? sender, EventArgs e)
    {
        AppCore.MainWindow.Width += 1;
        AppCore.MainWindow.Width -= 1;
    }
}