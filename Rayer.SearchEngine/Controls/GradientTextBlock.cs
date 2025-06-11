using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media;

namespace Rayer.SearchEngine.Controls;

public sealed class GradientTextBlock : FrameworkElement
{
    private FormattedText _formattedText = default!;
    private Size _lastRenderSize;
    private double _originalProgress;
    private static readonly Typeface _defaultTypeface = new("Microsoft YaHei");
    private static readonly Color _defaultBackgroundColor = Color.FromRgb(122, 122, 122);

    #region 依赖属性
    public static readonly DependencyProperty TextProperty =
        TextBlock.TextProperty.AddOwner(typeof(GradientTextBlock));

    public static readonly DependencyProperty ProgressProperty =
        DependencyProperty.Register("Progress", typeof(double), typeof(GradientTextBlock),
            new FrameworkPropertyMetadata(0.0, FrameworkPropertyMetadataOptions.AffectsRender));

    public static readonly DependencyProperty FontSizeProperty =
        TextElement.FontSizeProperty.AddOwner(typeof(GradientTextBlock));

    public static readonly DependencyProperty IsGradientableProperty =
        DependencyProperty.Register("IsGradientable", typeof(bool), typeof(GradientTextBlock),
            new FrameworkPropertyMetadata(false,
                FrameworkPropertyMetadataOptions.AffectsRender,
                OnIsGradientableChanged));
    #endregion

    static GradientTextBlock()
    {
        TextProperty.OverrideMetadata(typeof(GradientTextBlock),
            new FrameworkPropertyMetadata(
                string.Empty,
                FrameworkPropertyMetadataOptions.AffectsMeasure |
                FrameworkPropertyMetadataOptions.AffectsRender));

        FontSizeProperty.OverrideMetadata(typeof(GradientTextBlock),
            new FrameworkPropertyMetadata(
                24D,
                FrameworkPropertyMetadataOptions.AffectsMeasure |
                FrameworkPropertyMetadataOptions.AffectsRender));
    }

    #region 属性
    public string Text
    {
        get => (string)GetValue(TextProperty);
        set => SetValue(TextProperty, value);
    }

    public double Progress
    {
        get => (double)GetValue(ProgressProperty);
        set => SetValue(ProgressProperty, value);
    }

    public double FontSize
    {
        get => (double)GetValue(FontSizeProperty);
        set => SetValue(FontSizeProperty, value);
    }

    public bool IsGradientable
    {
        get => (bool)GetValue(IsGradientableProperty);
        set => SetValue(IsGradientableProperty, value);
    }
    #endregion

    protected override Size MeasureOverride(Size availableSize)
    {
        if (string.IsNullOrEmpty(Text))
        {
            return new Size(0, 0);
        }

        _formattedText = CreateFormattedText(Text, Brushes.White, 1920);
        return new Size(_formattedText.Width, _formattedText.Height);
    }

    protected override void OnRender(DrawingContext drawingContext)
    {
        base.OnRender(drawingContext);

        if (_formattedText is null || string.IsNullOrEmpty(Text))
        {
            return;
        }

        _lastRenderSize = new Size(ActualWidth, ActualHeight);

        if (IsGradientable)
        {
            // 计算每个字符的位置
            double xPos;
            var charWidths = new double[Text.Length];

            for (int i = 0; i < Text.Length; i++)
            {
                charWidths[i] = CreateFormattedText(Text[i].ToString(), Brushes.Black).Width;
            }

            // 绘制每个字符
            xPos = 0;
            for (int i = 0; i < Text.Length; i++)
            {
                double charProgress = Math.Min(1, Math.Max(0, (Progress * Text.Length) - i));

                // 创建渐变画笔（从白色到灰色）
                var gradientBrush = new LinearGradientBrush()
                {
                    StartPoint = new Point(0, 0),
                    EndPoint = new Point(1, 0)
                };

                gradientBrush.GradientStops.Add(new GradientStop(Colors.White, charProgress));
                gradientBrush.GradientStops.Add(new GradientStop(_defaultBackgroundColor, charProgress));

                // 绘制单个字符
                var charText = CreateFormattedText(Text[i].ToString(), gradientBrush);

                drawingContext.DrawText(charText, new Point(xPos, 0));
                xPos += charWidths[i];
            }
        }
        else
        {
            drawingContext.DrawText(_formattedText, new Point(0, 0));
        }
    }

    protected override void OnRenderSizeChanged(SizeChangedInfo sizeInfo)
    {
        base.OnRenderSizeChanged(sizeInfo);
        InvalidateVisual();
    }

    private FormattedText CreateFormattedText(string text, Brush foreground, double? maxWidth = null)
    {
        var formattedText = new FormattedText(
            text,
            CultureInfo.CurrentCulture,
            FlowDirection.LeftToRight,
            _defaultTypeface,
            FontSize,
            foreground,
            VisualTreeHelper.GetDpi(this).PixelsPerDip);

        if (maxWidth.HasValue)
        {
            formattedText.MaxTextWidth = maxWidth.Value;
            formattedText.Trimming = TextTrimming.CharacterEllipsis;
        }

        return formattedText;
    }

    private static void OnIsGradientableChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        var control = (GradientTextBlock)d;
        control.UpdatePauseState();
    }

    private void UpdatePauseState()
    {
        if (!IsGradientable)
        {
            // 暂停/拖动时保存原始状态            
            _originalProgress = Progress;

            // 清除渐变
            Progress = 0;
        }
        else
        {
            // 恢复时还原原始状态
            Progress = _originalProgress;
        }
    }
}