using Rayer.Core;
using Rayer.Core.Controls;
using Rayer.SearchEngine.Core.Domain.Aduio;
using Rayer.SearchEngine.ViewModels.Presenter;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;

namespace Rayer.SearchEngine.Controls.Search;

public partial class SearchAudioPresenter : AdaptiveUserControl, IPresenterControl<SearchAudioPresenterViewModel, SearchAudio>
{
    public SearchAudioPresenter(SearchAudioPresenterViewModel vm)
        : base(vm)
    {
        ViewModel = vm;
        DataContext = this;

        ViewModel.DataChanged += OnDataChanged;

        InitializeComponent();
    }

    public new SearchAudioPresenterViewModel ViewModel { get; set; }

    protected override void OnLoaded(object sender, RoutedEventArgs e)
    {
        ViewModel ??= AppCore.GetRequiredService<SearchAudioPresenterViewModel>();
        base.ViewModel = ViewModel;

        base.OnLoaded(sender, e);
    }

    protected override void OnUnLoaded(object sender, RoutedEventArgs e)
    {
        base.OnUnLoaded(sender, e);

        ViewModel = default!;

        BindingOperations.ClearAllBindings(this);

        GC.Collect();
    }

    private void OnListViewItemRightButtonUp(object sender, System.Windows.Input.MouseButtonEventArgs e)
    {

    }

    private async void OnListViewItemDoubleClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
    {
        if (e.Source is ListViewItem listViewItem &&
            listViewItem.DataContext is SearchAudioDetail item)
        {
            await ViewModel.PlayWebAudio(item);
        }
    }

    private void OnDataChanged(object? sender, EventArgs e)
    {
        AppCore.MainWindow.Width += 1;
        AppCore.MainWindow.Width -= 1;
    }
}