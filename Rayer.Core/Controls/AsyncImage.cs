using Rayer.Core.Utils;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace Rayer.Core.Controls;

public class AsyncImage : Control
{
    #region Property Definition
    public static readonly DependencyProperty SourceProperty = DependencyProperty.Register(
        nameof(Source),
        typeof(ImageSource),
        typeof(AsyncImage),
        new FrameworkPropertyMetadata(
            null,
            FrameworkPropertyMetadataOptions.AffectsMeasure | FrameworkPropertyMetadataOptions.AffectsRender,
            null,
            null
        ),
        null
    );

    public static readonly DependencyProperty UriSourceProperty = DependencyProperty.Register(
        nameof(UriSource),
        typeof(Uri),
        typeof(AsyncImage),
        new FrameworkPropertyMetadata(
            null,
            FrameworkPropertyMetadataOptions.AffectsMeasure | FrameworkPropertyMetadataOptions.AffectsRender,
            new PropertyChangedCallback(OnUriSourceChanged),
            null
        ),
        null
    );

    public static readonly DependencyProperty FallbackProperty = DependencyProperty.Register(
        nameof(Fallback),
        typeof(ImageSource),
        typeof(AsyncImage),
        new FrameworkPropertyMetadata(
            null,
            FrameworkPropertyMetadataOptions.AffectsMeasure | FrameworkPropertyMetadataOptions.AffectsRender,
            null,
            null
        ),
        null
    );

    public static readonly DependencyProperty StretchProperty = DependencyProperty.Register(
        nameof(Stretch),
        typeof(Stretch),
        typeof(AsyncImage),
        new FrameworkPropertyMetadata(
            Stretch.Uniform,
            FrameworkPropertyMetadataOptions.AffectsMeasure | FrameworkPropertyMetadataOptions.AffectsRender
        ),
        null
    );

    public static ImageSource GetSource(DependencyObject obj)
    {
        return (ImageSource)obj.GetValue(SourceProperty);
    }

    public static void SetSource(DependencyObject obj, ImageSource value)
    {
        obj.SetValue(SourceProperty, value);
    }

    public static Uri GetUriSource(DependencyObject obj)
    {
        return (Uri)obj.GetValue(UriSourceProperty);
    }

    public static void SetUriSource(DependencyObject obj, Uri value)
    {
        obj.SetValue(UriSourceProperty, value);
    }

    public static ImageSource GetFallback(DependencyObject obj)
    {
        return (ImageSource)obj.GetValue(FallbackProperty);
    }

    public static void SetFallback(DependencyObject obj, ImageSource value)
    {
        obj.SetValue(FallbackProperty, value);
    }
    #endregion

    public AsyncImage()
    {
        Loaded += OnLoaded;
        Unloaded += OnUnloaded;
    }

    public ImageSource? Source
    {
        get => (ImageSource?)GetValue(SourceProperty);
        set => SetValue(SourceProperty, value);
    }
    
    public Uri UriSource
    {
        get => (Uri)GetValue(UriSourceProperty);
        set => SetValue(UriSourceProperty, value);
    }

    public ImageSource? Fallback
    {
        get => (ImageSource?)GetValue(FallbackProperty);
        set => SetValue(FallbackProperty, value);
    }

    public Stretch Stretch
    {
        get => (Stretch)GetValue(StretchProperty);
        set => SetValue(StretchProperty, value);
    }

    public CornerRadius CornerRadius
    {
        get => (CornerRadius)GetValue(CornerRadiusProperty);
        set => SetValue(CornerRadiusProperty, value);
    }

    internal CornerRadius InnerCornerRadius => (CornerRadius)GetValue(InnerCornerRadiusProperty);

    public static readonly DependencyProperty CornerRadiusProperty = DependencyProperty.Register(
        nameof(CornerRadius),
        typeof(CornerRadius),
        typeof(AsyncImage),
        new PropertyMetadata(new CornerRadius(0), new PropertyChangedCallback(OnCornerRadiusChanged))
    );

    public static readonly DependencyPropertyKey InnerCornerRadiusPropertyKey =
        DependencyProperty.RegisterReadOnly(
            nameof(InnerCornerRadius),
            typeof(CornerRadius),
            typeof(AsyncImage),
            new PropertyMetadata(new CornerRadius(0))
        );

    public static readonly DependencyProperty InnerCornerRadiusProperty =
        InnerCornerRadiusPropertyKey.DependencyProperty;

    private static async void OnUriSourceChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        var fallback = GetFallback(d);

        if (fallback is not null)
        {
            SetSource(d, fallback);
        }

        if (e.NewValue is Uri uri && e.OldValue is null)
        {
            var image = await ImageSourceFactory.CreateWebSourceAsync(uri);

            SetSource(d, image);
        }
        else if (e.NewValue is Uri newUri && e.OldValue is Uri oldUri && newUri.AbsoluteUri != oldUri.AbsoluteUri)
        {
            var image = await ImageSourceFactory.CreateWebSourceAsync(newUri);

            SetSource(d, image);
        }
    }

    private static void OnCornerRadiusChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        var thickness = (Thickness)d.GetValue(BorderThicknessProperty);
        var outerRarius = (CornerRadius)e.NewValue;

        d.SetValue(
            InnerCornerRadiusPropertyKey,
            new CornerRadius(
                topLeft: Math.Max(0, (int)Math.Round(outerRarius.TopLeft - (thickness.Left / 2), 0)),
                topRight: Math.Max(0, (int)Math.Round(outerRarius.TopRight - (thickness.Top / 2), 0)),
                bottomRight: Math.Max(0, (int)Math.Round(outerRarius.BottomRight - (thickness.Right / 2), 0)),
                bottomLeft: Math.Max(0, (int)Math.Round(outerRarius.BottomLeft - (thickness.Bottom / 2), 0))
            )
        );
    }

    private void OnLoaded(object sender, RoutedEventArgs e)
    {
        var fallback = GetFallback(this);

        if (fallback is not null)
        {
            SetSource(this, fallback);
        }
    }

    private void OnUnloaded(object sender, RoutedEventArgs e)
    {
        Loaded -= OnLoaded;
        Unloaded -= OnUnloaded;
    }
}