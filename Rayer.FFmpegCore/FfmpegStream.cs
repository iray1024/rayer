using System.Runtime.InteropServices;

namespace Rayer.FFmpegCore;

internal sealed class FfmpegStream : IDisposable
{
    public Stream _stream;

    public AvioContext AvioContext { get; private set; }

    public FfmpegStream(Stream stream) : this(stream, true)
    {

    }

    public FfmpegStream(Stream stream, bool allowWrite)
    {
        ArgumentNullException.ThrowIfNull(stream);

        if (!stream.CanRead)
        {
            throw new ArgumentException("Á÷²»¿É¶Á¡£", nameof(stream));
        }

        _stream = stream;

        AvioContext = new AvioContext(ReadDataCallback,
            stream.CanSeek ? new FFmpegCalls.AvioSeek(SeekCallback) : null!,
            stream.CanWrite && allowWrite ? new FFmpegCalls.AvioWriteData(WriteDataCallback) : null!);
    }

    private long SeekCallback(nint opaque, long offset, FFmpegCalls.SeekFlags whence)
    {
        if ((whence & FFmpegCalls.SeekFlags.SeekSize) == FFmpegCalls.SeekFlags.SeekSize)
        {
            return _stream.Length;
        }

        SeekOrigin origin;
        if ((whence & FFmpegCalls.SeekFlags.SeekSet) == FFmpegCalls.SeekFlags.SeekSet)
        {
            origin = SeekOrigin.Begin;
        }
        else if ((whence & FFmpegCalls.SeekFlags.SeekCur) == FFmpegCalls.SeekFlags.SeekCur)
        {
            origin = SeekOrigin.Current;
        }
        else if ((whence & FFmpegCalls.SeekFlags.SeekEnd) == FFmpegCalls.SeekFlags.SeekEnd)
        {
            origin = SeekOrigin.End;
        }
        else
        {
            return -1;
        }

        try
        {
            return _stream.Seek(offset, origin);
        }
        catch (Exception)
        {
            return -1;
        }
    }

    private int WriteDataCallback(nint opaque, nint buffer, int bufferSize)
    {
        var managedBuffer = new byte[bufferSize];

        Marshal.Copy(buffer, managedBuffer, 0, bufferSize);
        _stream.Write(managedBuffer, 0, bufferSize);

        return bufferSize;
    }

    private int ReadDataCallback(nint opaque, nint buffer, int bufferSize)
    {
        var managedBuffer = new byte[bufferSize];
        var read = 0;
        while (read < bufferSize)
        {
            var read0 = _stream.Read(managedBuffer, read, bufferSize - read);
            read += read0;
            if (read0 == 0)
            {
                break;
            }
        }

        read = Math.Min(read, bufferSize);
        Marshal.Copy(managedBuffer, 0, buffer, Math.Min(read, bufferSize));

        Console.WriteLine("Read: " + read);
        return read;
    }

    public void Dispose()
    {
        GC.SuppressFinalize(this);

        if (AvioContext != null)
        {
            AvioContext.Dispose();
            AvioContext = null!;
        }
    }

    ~FfmpegStream()
    {
        Dispose();
    }
}