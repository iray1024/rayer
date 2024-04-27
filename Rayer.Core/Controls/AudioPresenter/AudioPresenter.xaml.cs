using Rayer.Core.Utils;
using System.ComponentModel;
using System.Windows;
using System.Windows.Media;

namespace Rayer.Core.Controls;

public partial class AudioPresenter : System.Windows.Controls.UserControl
{
    #region Properties
    public static readonly DependencyProperty IsSelectedProperty =
        DependencyProperty.Register(
            "IsSelected",
            typeof(bool),
            typeof(AudioPresenter),
            new PropertyMetadata(false, OnIsSelectedPropertyChanged));

    [Bindable(true)]
    public bool IsSelected
    {
        get => (bool)GetValue(IsSelectedProperty);
        set => SetValue(IsSelectedProperty, value);
    }

    public static readonly DependencyProperty IsAvailableProperty =
        DependencyProperty.Register(
            "IsAvailable",
            typeof(bool),
            typeof(AudioPresenter),
            new PropertyMetadata(true, OnIsAvailablePropertyChanged));

    [Bindable(true)]
    public bool IsAvailable
    {
        get => (bool)GetValue(IsAvailableProperty);
        set => SetValue(IsAvailableProperty, value);
    }

    public static readonly DependencyProperty AlbumWidthProperty =
            DependencyProperty.Register(
                "AlbumWidth",
                typeof(double),
                typeof(AudioPresenter),
                new PropertyMetadata(36d));

    public double AlbumWidth
    {
        get => (double)GetValue(AlbumWidthProperty);
        set => SetValue(AlbumWidthProperty, value);
    }

    public static readonly DependencyProperty TitleMaxWidthProperty =
        DependencyProperty.Register(
            "TitleMaxWidth",
            typeof(double),
            typeof(AudioPresenter),
            new PropertyMetadata(double.NaN));

    public double TitleMaxWidth
    {
        get => (double)GetValue(TitleMaxWidthProperty);
        set => SetValue(TitleMaxWidthProperty, value);
    }

    public static readonly DependencyProperty ArtistsMaxWidthProperty =
        DependencyProperty.Register(
            "ArtistsMaxWidth",
            typeof(double),
            typeof(AudioPresenter),
            new PropertyMetadata(double.NaN));

    public double ArtistsMaxWidth
    {
        get => (double)GetValue(ArtistsMaxWidthProperty);
        set => SetValue(ArtistsMaxWidthProperty, value);
    }

    public static readonly DependencyProperty AlbumTitleMaxWidthProperty =
        DependencyProperty.Register(
            "AlbumTitleMaxWidth",
            typeof(double),
            typeof(AudioPresenter),
            new PropertyMetadata(double.NaN));

    public double AlbumTitleMaxWidth
    {
        get => (double)GetValue(AlbumTitleMaxWidthProperty);
        set => SetValue(AlbumTitleMaxWidthProperty, value);
    }

    public static readonly DependencyProperty DurationMaxWidthProperty =
        DependencyProperty.Register(
            "DurationMaxWidth",
            typeof(double),
            typeof(AudioPresenter),
            new PropertyMetadata(double.NaN));

    public double DurationMaxWidth
    {
        get => (double)GetValue(DurationMaxWidthProperty);
        set => SetValue(DurationMaxWidthProperty, value);
    }

    public static readonly DependencyProperty AlbumProperty =
        DependencyProperty.Register(
            "Album",
            typeof(string),
            typeof(AudioPresenter),
            new PropertyMetadata(null));

    [Bindable(true)]
    public string Album
    {
        get => (string)GetValue(AlbumProperty);
        set => SetValue(AlbumProperty, value);
    }

    public static readonly DependencyProperty AlbumSourceProperty =
        DependencyProperty.Register(
            "AlbumSource",
            typeof(ImageSource),
            typeof(AudioPresenter),
            new PropertyMetadata(null));

    [Bindable(true)]
    public ImageSource AlbumSource
    {
        get => (ImageSource)GetValue(AlbumSourceProperty);
        set => SetValue(AlbumSourceProperty, value);
    }

    public static readonly DependencyProperty TitleProperty =
        DependencyProperty.Register(
            "Title",
            typeof(string),
            typeof(AudioPresenter),
            new PropertyMetadata(null));

    [Bindable(true)]
    public string Title
    {
        get => (string)GetValue(TitleProperty);
        set => SetValue(TitleProperty, value);
    }

    public static readonly DependencyProperty ArtistsProperty =
        DependencyProperty.Register(
            "Artists",
            typeof(string),
            typeof(AudioPresenter),
            new PropertyMetadata(null));

    [Bindable(true)]
    public string Artists
    {
        get => (string)GetValue(ArtistsProperty);
        set => SetValue(ArtistsProperty, value);
    }

    public static readonly DependencyProperty AlbumTitleProperty =
        DependencyProperty.Register(
            "AlbumTitle",
            typeof(string),
            typeof(AudioPresenter),
            new PropertyMetadata(null, OnAlbumTitlePropertyChanged));

    [Bindable(true)]
    public string AlbumTitle
    {
        get => (string)GetValue(AlbumTitleProperty);
        set => SetValue(AlbumTitleProperty, value);
    }

    public static readonly DependencyProperty DurationProperty =
        DependencyProperty.Register(
            "Duration",
            typeof(string),
            typeof(AudioPresenter),
            new PropertyMetadata(null, OnDurationPropertyChanged));

    [Bindable(true)]
    public string Duration
    {
        get => (string)GetValue(DurationProperty);
        set => SetValue(DurationProperty, value);
    }

    public static readonly DependencyProperty CopyrightProperty =
        DependencyProperty.Register(
            "Copyright",
            typeof(string),
            typeof(AudioPresenter),
            new PropertyMetadata(null, OnCopyrightPropertyChanged));

    [Bindable(true)]
    public string Copyright
    {
        get => (string)GetValue(CopyrightProperty);
        set => SetValue(CopyrightProperty, value);
    }
    #endregion

    public AudioPresenter()
    {
        InitializeComponent();
    }

    private static void OnIsAvailablePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        var listViewItem = ElementHelper.FindParent<ListViewItem>(d);

        listViewItem?.SetValue(ListViewItem.IsAvailableProperty, e.NewValue);
    }

    private static void OnIsSelectedPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        var listViewItem = ElementHelper.FindParent<ListViewItem>(d);

        listViewItem?.SetValue(ListViewItem.IsSelectedProperty, e.NewValue);
    }

    private static void OnAlbumTitlePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        var instance = (AudioPresenter)d;

        instance.PART_AlbumTitle.Visibility = e.NewValue is not null ? Visibility.Visible : Visibility.Collapsed;
    }

    private static void OnDurationPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        var instance = (AudioPresenter)d;

        instance.PART_Duration.Visibility = e.NewValue is not null ? Visibility.Visible : Visibility.Collapsed;
    }

    private static void OnCopyrightPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        var instance = (AudioPresenter)d;

        if (e.NewValue is string { Length: > 0 })
        {
            System.Windows.Controls.ToolTipService.SetToolTip(instance.PART_Album, e.NewValue);
        }
    }
}