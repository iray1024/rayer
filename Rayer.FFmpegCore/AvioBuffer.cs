namespace Rayer.FFmpegCore;

internal sealed class AvioBuffer : IDisposable
{
    public AvioBuffer()
        : this(0x1000)
    {

    }

    public AvioBuffer(int bufferSize)
    {
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(bufferSize);

        BufferSize = bufferSize;
        Buffer = FFmpegCalls.AvMalloc(bufferSize);
        SuppressAvFree = false;
    }

    public int BufferSize { get; private set; }

    public nint Buffer { get; private set; }

    public bool SuppressAvFree { get; set; }

    public void Dispose()
    {
        GC.SuppressFinalize(this);
        if (SuppressAvFree != true)
        {
            FFmpegCalls.AvFree(Buffer);
        }
    }

    ~AvioBuffer()
    {
        Dispose();
    }
}