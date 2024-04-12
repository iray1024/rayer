using Rayer.FFmpegCore.Interops;

namespace Rayer.FFmpegCore;

internal sealed class AvioContext : IDisposable
{
    private readonly FFmpegCalls.AvioReadData _readDataCallback;
    private readonly FFmpegCalls.AvioSeek _seekCallback;
    private readonly FFmpegCalls.AvioWriteData _writeDataCallback = null!;
    private unsafe AVIOContext* _context;
    private readonly AvioBuffer _buffer;

    public AvioContext(
        FFmpegCalls.AvioReadData readDataCallback,
        FFmpegCalls.AvioSeek seekCallback)
        : this(readDataCallback, seekCallback, default!)
    {
    }

    public unsafe AvioContext(
        FFmpegCalls.AvioReadData readDataCallback,
        FFmpegCalls.AvioSeek seekCallback,
        FFmpegCalls.AvioWriteData writeDataCallback)
    {
        _readDataCallback = readDataCallback;
        _seekCallback = seekCallback;
        _writeDataCallback = writeDataCallback;

        _buffer = new AvioBuffer { SuppressAvFree = true };
        _context = FFmpegCalls.AvioAllocContext(_buffer, _writeDataCallback != null, nint.Zero,
            _readDataCallback, _writeDataCallback!, _seekCallback);
    }

    public unsafe nint ContextPtr => (nint)_context;

    public unsafe AVIOContext Context => _context == null ? default : *_context;

    public unsafe void Dispose()
    {
        GC.SuppressFinalize(this);
        if (_context != null)
        {
            FFmpegCalls.AvFree((nint)_context->buffer);
            FFmpegCalls.AvFree((nint)_context);

            _context = null;
        }
    }

    ~AvioContext()
    {
        Dispose();
    }
}