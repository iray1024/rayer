using Rayer.Core.Lyric.Abstractions;
using Rayer.Core.Lyric.Impl;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace Rayer.SearchEngine.Controls;

public sealed class GradientTextBlock : Control
{
    private readonly List<TextInfo> _textInfos = [];
    private double _originalProgress;
    private static readonly Typeface _defaultTypeface = new("Microsoft YaHei");
    private static readonly Color _defaultBackgroundColor = Color.FromRgb(122, 122, 122);

    #region 依赖属性
    public static readonly DependencyProperty TextProperty =
        TextBlock.TextProperty.AddOwner(typeof(GradientTextBlock));

    public static readonly DependencyProperty ProgressProperty =
        DependencyProperty.Register("Progress", typeof(double), typeof(GradientTextBlock),
            new FrameworkPropertyMetadata(0.0, FrameworkPropertyMetadataOptions.AffectsRender));

    public static readonly DependencyProperty IsGradientableProperty =
        DependencyProperty.Register("IsGradientable", typeof(bool), typeof(GradientTextBlock),
            new FrameworkPropertyMetadata(false,
                FrameworkPropertyMetadataOptions.AffectsRender,
                OnIsGradientableChanged));

    public static readonly DependencyProperty LyricsProperty =
        DependencyProperty.Register("Lyrics", typeof(ILineInfo), typeof(GradientTextBlock),
            new PropertyMetadata(null, OnLyricsChanged));
    #endregion

    static GradientTextBlock()
    {
        TextProperty.OverrideMetadata(typeof(GradientTextBlock),
            new FrameworkPropertyMetadata(
                string.Empty,
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

    public bool IsGradientable
    {
        get => (bool)GetValue(IsGradientableProperty);
        set => Dispatcher.BeginInvoke(() => SetValue(IsGradientableProperty, value));
    }

    public ILineInfo Lyrics
    {
        get => (ILineInfo)GetValue(LyricsProperty);
        set => SetValue(LyricsProperty, value);
    }

    public TimeSpan CurrentTime { get; set; }
    #endregion

    protected override Size MeasureOverride(Size availableSize)
    {
        if (_textInfos.Count == 0)
        {
            return new Size(0, 32);
        }

        double totalWidth = 0;
        double maxHeight = 0;

        foreach (var textInfo in _textInfos)
        {
            totalWidth += textInfo.Width;
            maxHeight = Math.Max(maxHeight, textInfo.Height);
        }

        return new Size(Math.Min(totalWidth, availableSize.Width), Math.Min(maxHeight, 1920));
    }

    protected override void OnRender(DrawingContext drawingContext)
    {
        base.OnRender(drawingContext);

        if (_textInfos.Count == 0)
        {
            return;
        }

        if (_textInfos.Count > 1 && IsGradientable)
        {
            _textInfos.ForEach(e => e.CurrentIsGradientable = IsGradientable);
        }

        double xPos = 0;
        foreach (var textInfo in _textInfos)
        {
            if (textInfo.CurrentIsGradientable)
            {
                var charProgress = CalculateProgress(textInfo.StartTime, textInfo.EndTime, unchecked((int)CurrentTime.TotalMilliseconds));

                // 创建渐变画笔（从白色到灰色）
                var gradientBrush = new LinearGradientBrush()
                {
                    StartPoint = new Point(0, 0),
                    EndPoint = new Point(1, 0)
                };

                gradientBrush.GradientStops.Add(new GradientStop(Colors.White, charProgress));
                gradientBrush.GradientStops.Add(new GradientStop(_defaultBackgroundColor, charProgress));

                // 绘制单个字符                
                var formattedText = textInfo.FormattedText;
                formattedText.SetForegroundBrush(gradientBrush);

                drawingContext.DrawText(formattedText, new Point(xPos, 0));
                xPos += textInfo.Width;
            }
            else
            {
                drawingContext.DrawText(_textInfos.First().FormattedText, new Point(0, 0));
            }
        }
    }

    protected override void OnRenderSizeChanged(SizeChangedInfo sizeInfo)
    {
        base.OnRenderSizeChanged(sizeInfo);
        InvalidateVisual();
    }

    private static void OnIsGradientableChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        var control = (GradientTextBlock)d;
        control.UpdatePauseState();
    }

    private static void OnLyricsChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        var control = (GradientTextBlock)d;
        control.InitializeText();
        control.InvalidateVisual();
    }

    private void InitializeText()
    {
        _textInfos.Clear();

        if (Lyrics is not SyllableLineInfo syllableLineInfo || (syllableLineInfo is not { IsSyllable: true }))
        {
            var formattedText = CreateFormattedText(Lyrics?.Text ?? Text, Brushes.White);
            _textInfos.Add(new TextInfo
            {
                FormattedText = formattedText,
                Width = formattedText.Width,
                Height = formattedText.Height,
                CurrentIsGradientable = IsGradientable
            });

            return;
        }

        foreach (var syllable in syllableLineInfo.Syllables)
        {
            var formattedText = CreateFormattedText(syllable.Text, Brushes.White);
            _textInfos.Add(new TextInfo
            {
                FormattedText = formattedText,
                StartTime = syllable.StartTime,
                EndTime = syllable.EndTime,
                Width = formattedText.Width,
                Height = formattedText.Height,
                CurrentIsGradientable = IsGradientable
            });
        }
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

    private static double CalculateProgress(int startTime, int endTime, int currentTime)
    {
        if (currentTime <= startTime)
        {
            return 0;
        }

        if (currentTime >= endTime)
        {
            return 1;
        }

        return (double)(currentTime - startTime) / (endTime - startTime);
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

    private class TextInfo
    {
        public FormattedText FormattedText { get; set; } = null!;

        public int StartTime { get; set; }

        public int EndTime { get; set; }

        public double Width { get; set; }

        public double Height { get; set; }

        public required bool CurrentIsGradientable { get; set; }
    }
}