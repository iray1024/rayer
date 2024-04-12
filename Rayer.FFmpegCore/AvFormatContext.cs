using Rayer.FFmpegCore.Interops;

namespace Rayer.FFmpegCore;

internal class AvFormatContext : IDisposable
{
    private unsafe AVFormatContext* _formatContext;
    private AvStream _stream = null!;

    public unsafe AvFormatContext(FfmpegStream stream)
    {
        _formatContext = FFmpegCalls.AvformatAllocContext();

        fixed (AVFormatContext** pformatContext = &_formatContext)
        {
            FFmpegCalls.AvformatOpenInput(pformatContext, stream.AvioContext);
        }

        Initialize();
    }

    public unsafe AvFormatContext(string url)
    {
        _formatContext = FFmpegCalls.AvformatAllocContext();

        fixed (AVFormatContext** pformatContext = &_formatContext)
        {
            FFmpegCalls.AvformatOpenInput(pformatContext, url);
        }

        Initialize();
    }

    public unsafe nint FormatPtr => (nint)_formatContext;

    public int BestAudioStreamIndex { get; private set; }

    public AvStream SelectedStream => _stream;

    public unsafe bool CanSeek
        => _formatContext != null && _formatContext->pb != null && _formatContext->pb->seekable == 1;

    public double LengthInSeconds
    {
        get
        {
            if (SelectedStream == null || SelectedStream.Stream.duration < 0)
            {
                return 0;
            }

            var timebase = SelectedStream?.Stream.time_base;
            return (timebase?.den == 0 ? 0 : SelectedStream?.Stream.duration * timebase?.num / (double)(timebase?.den ?? 0d)) ?? 0;
        }
    }

    public unsafe AVFormatContext FormatContext
        => _formatContext == null ? default : *_formatContext;

    public Dictionary<string, string> Metadata { get; private set; } = [];

    private unsafe void Initialize()
    {
        FFmpegCalls.AvFormatFindStreamInfo(_formatContext);

        BestAudioStreamIndex = FFmpegCalls.AvFindBestStreamInfo(_formatContext);
        _stream = new AvStream((nint)_formatContext->streams[BestAudioStreamIndex]);

        Metadata = [];
        if (_formatContext->metadata != null)
        {
            var metadata = _formatContext->metadata->Elements;
            foreach (var element in metadata)
            {
                Metadata.Add(element.Key, element.Value);
            }
        }
    }

    public void SeekFile(double seconds)
    {
        var streamTimeBase = SelectedStream.Stream.time_base;
        var time = seconds * streamTimeBase.den / streamTimeBase.num;

        FFmpegCalls.AvFormatSeekFile(this, time);
    }

    public unsafe void Dispose()
    {
        GC.SuppressFinalize(this);

        if (SelectedStream != null)
        {
            SelectedStream.Dispose();
            _stream = null!;
        }

        if (_formatContext != null)
        {
            fixed (AVFormatContext** pformatContext = &_formatContext)
            {
                FFmpegCalls.AvformatCloseInput(pformatContext);
            }

            _formatContext = null;
            BestAudioStreamIndex = 0;
        }
    }

    ~AvFormatContext()
    {
        Dispose();
    }
}