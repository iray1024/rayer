using Rayer.Core;
using Rayer.Core.Abstractions;
using Rayer.Core.Controls;
using Rayer.Core.Framework.Injection;
using Rayer.ViewModels;
using System.Windows;
using Wpf.Ui.Controls;

namespace Rayer.Views.Pages;

[Inject]
public partial class PlaylistPage : AdaptivePage, INavigableView<PlaylistPageViewModel>, INavigationAware
{
    private readonly IAudioManager _audioManager;

    private int _hasNavigationTo = 0;

    public PlaylistPage(
        PlaylistPageViewModel viewModel,
        IAudioManager audioManager)
        : base(viewModel)
    {
        _audioManager = audioManager;
        _audioManager.AudioChanged += OnAudioChanged;
        _audioManager.AudioStopped += OnAudioStopped;

        ViewModel = viewModel;
        DataContext = this;

        InitializeComponent();
    }

    public bool HasNavigationTo
    {
        get => _hasNavigationTo == 1;
        set => _ = Interlocked.Exchange(ref _hasNavigationTo, value ? 1 : 0);
    }

    public new PlaylistPageViewModel ViewModel { get; set; }

    protected override void OnLoaded(object sender, RoutedEventArgs e)
    {
        ViewModel ??= AppCore.GetRequiredService<PlaylistPageViewModel>();
        base.ViewModel = ViewModel;

        base.OnLoaded(sender, e);
    }

    private void OnAudioChanged(object? sender, Core.Events.AudioChangedArgs e)
    {

    }

    private void OnAudioStopped(object? sender, EventArgs e)
    {

    }

    private void OnListViewItemDoubleClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
    {

    }

    private void OnListViewItemRightButtonUp(object sender, System.Windows.Input.MouseButtonEventArgs e)
    {

    }

    public void OnNavigatedTo()
    {
        if (!HasNavigationTo)
        {
            HasNavigationTo = true;
        }
    }

    public void OnNavigatedFrom()
    {
        if (HasNavigationTo)
        {
            HasNavigationTo = false;
        }
    }
}