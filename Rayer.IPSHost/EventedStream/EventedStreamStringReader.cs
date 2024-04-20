namespace Rayer.IPSHost.EventedStream;

internal class EventedStreamStringReader : IDisposable
{
    private readonly EventedStreamReader _eventedStreamReader;
    private readonly StringBuilder _sb = new();

    private bool _isDisposed;

    public EventedStreamStringReader(EventedStreamReader eventedStreamReader)
    {
        ArgumentNullException.ThrowIfNull(eventedStreamReader);

        _eventedStreamReader = eventedStreamReader;

        _eventedStreamReader.OnReceivedLine += OnReceivedLine;
    }

    public string ReadAsString() => _sb.ToString();

    protected virtual void OnReceivedLine(string line)
        => _sb.AppendLine(line);

    public void Dispose()
    {
        if (!_isDisposed)
        {
            _eventedStreamReader.OnReceivedLine -= OnReceivedLine;
            _isDisposed = true;
        }
    }
}