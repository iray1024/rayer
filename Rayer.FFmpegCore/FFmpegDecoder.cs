using Rayer.FFmpegCore.Modules;
using Rayer.FFmpegCore.Modules.Abstractions;
using Rayer.FFmpegCore.Modules.Extensions;

namespace Rayer.FFmpegCore;

public class FFmpegDecoder : IWaveSource
{
    private readonly object _lockObject = new();
    private readonly Uri _uri = null!;
    private FfmpegStream _ffmpegStream = null!;
    private AvFormatContext _formatContext = null!;
    private bool _disposeStream = false;

    private byte[] _overflowBuffer = [];
    private int _overflowCount;
    private int _overflowOffset;
    private long _position;
    private Stream _stream = null!;

    public FFmpegDecoder(string url)
    {
        const int invalidArgument = unchecked((int)0xffffffea);

        _uri = new Uri(url);
        try
        {
            _formatContext = new AvFormatContext(url);

            Initialize();
        }
        catch (FFmpegException ex)
        {
            if (ex.ErrorCode == invalidArgument && "avformat_open_input".Equals(ex.Function, StringComparison.OrdinalIgnoreCase))
            {
                if (!TryInitializeWithFileAsStream(url))
                {
                    throw;
                }
            }
            else
            {
                throw;
            }
        }
    }

    public FFmpegDecoder(Stream stream)
    {
        ArgumentNullException.ThrowIfNull(stream);

        InitializeWithStream(stream, false);
    }

    public Dictionary<string, string> Metadata => _formatContext == null ? ([]) : _formatContext.Metadata;

    public bool CanSeek => _formatContext != null && _formatContext.CanSeek;

    public WaveFormat WaveFormat { get; private set; } = null!;

    public long Position
    {
        get => _position;
        set
        {
            SeekPosition(value);
        }
    }

    public long Length
        => _formatContext is null || _formatContext.SelectedStream is null
                ? 0
                : this.GetRawElements(TimeSpan.FromSeconds(_formatContext.LengthInSeconds));

    public int Read(byte[] buffer, int offset, int count)
    {
        var read = 0;
        count -= count % WaveFormat.BlockAlign;
        var fetchedOverflows = GetOverflows(buffer, ref offset, count);
        read += fetchedOverflows;

        while (read < count)
        {
            long packetPosition;
            int bufferLength;
            lock (_lockObject)
            {
                using var frame = new AvFrame(_formatContext);

                bufferLength = frame.ReadNextFrame(out var seconds, ref _overflowBuffer);
                packetPosition = this.GetRawElements(TimeSpan.FromSeconds(seconds));
            }
            if (bufferLength <= 0)
            {
                if (_uri != null && !_uri.IsFile)
                {
                    Thread.Sleep(10);
                }
                else
                {
                    break;
                }
            }

            var bytesToCopy = Math.Min(count - read, bufferLength);

            Array.Copy(_overflowBuffer, 0, buffer, offset, bytesToCopy);

            read += bytesToCopy;
            offset += bytesToCopy;

            _overflowCount = bufferLength > bytesToCopy ? bufferLength - bytesToCopy : 0;
            _overflowOffset = bufferLength > bytesToCopy ? bytesToCopy : 0;

            _position = packetPosition + read - fetchedOverflows;
        }

        if (fetchedOverflows == read)
        {
            _position += read;
        }

        return read;
    }

    private void Initialize()
    {
        WaveFormat = _formatContext.SelectedStream.GetSuggestedWaveFormat();
    }

    private void InitializeWithStream(Stream stream, bool disposeStream)
    {
        _stream = stream;
        _disposeStream = disposeStream;

        _ffmpegStream = new FfmpegStream(stream, false);
        _formatContext = new AvFormatContext(_ffmpegStream);

        Initialize();
    }

    private bool TryInitializeWithFileAsStream(string filename)
    {
        if (!File.Exists(filename))
        {
            return false;
        }

        Stream? stream = null;
        try
        {
            stream = File.OpenRead(filename);
            InitializeWithStream(stream, true);

            return true;
        }
        catch (Exception)
        {
            stream?.Dispose();

            return false;
        }
    }

    private void SeekPosition(long position)
    {
        var seconds = this.GetMilliseconds(position) / 1000.0;

        lock (_lockObject)
        {
            _formatContext.SeekFile(seconds);

            _position = position;
            _overflowCount = 0;
            _overflowOffset = 0;
        }
    }

    private int GetOverflows(byte[] buffer, ref int offset, int count)
    {
        if (_overflowCount != 0 && _overflowBuffer != null && count > 0)
        {
            var bytesToCopy = Math.Min(count, _overflowCount);

            Array.Copy(_overflowBuffer, _overflowOffset, buffer, offset, bytesToCopy);

            _overflowCount -= bytesToCopy;
            _overflowOffset += bytesToCopy;
            offset += bytesToCopy;

            return bytesToCopy;
        }

        return 0;
    }

    public void Dispose()
    {
        GC.SuppressFinalize(this);
        Dispose(true);
    }

    protected void Dispose(bool disposing)
    {
        if (disposing)
        {
            if (_disposeStream && _stream != null)
            {
                _stream.Dispose();
                _stream = null!;
            }

            if (_formatContext != null)
            {
                _formatContext.Dispose();
                _formatContext = null!;
            }

            if (_ffmpegStream != null)
            {
                _ffmpegStream.Dispose();
                _ffmpegStream = null!;
            }
        }
    }

    ~FFmpegDecoder()
    {
        Dispose(false);
    }
}