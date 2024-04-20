using System.Text.RegularExpressions;

namespace Rayer.IPSHost.EventedStream;

internal class EventedStreamReader
{
    public delegate void OnReceivedChunkHandler(ArraySegment<char> chunk);
    public delegate void OnReceivedLineHandler(string line);
    public delegate void OnStreamClosedHandler();

    private readonly StreamReader _streamReader;
    private readonly StringBuilder _linesBuffer;

    public EventedStreamReader(StreamReader streamReader)
    {
        _streamReader = streamReader ?? throw new ArgumentNullException(nameof(streamReader));
        _linesBuffer = new StringBuilder();
        Task.Factory.StartNew(Run);
    }

    public event OnReceivedChunkHandler? OnReceivedChunk;
    public event OnReceivedLineHandler? OnReceivedLine;
    public event OnStreamClosedHandler? OnStreamClosed;

    public Task<Match> WaitForMatch(Regex regex)
    {
        var tcs = new TaskCompletionSource<Match>();

        var completionLock = new object();

        OnReceivedLineHandler onReceivedLineHandler = default!;
        OnStreamClosedHandler onStreamClosedHandler = default!;

        void ResolveIfStillPending(Action applyResolution)
        {
            lock (completionLock)
            {
                if (!tcs.Task.IsCompleted)
                {
                    OnReceivedLine -= onReceivedLineHandler;
                    OnStreamClosed -= onStreamClosedHandler;

                    applyResolution();
                }
            }
        }

        onReceivedLineHandler = line =>
        {
            var match = regex.Match(line);

            if (match.Success)
            {
                ResolveIfStillPending(() =>
                    tcs.SetResult(match));
            }
        };

        onStreamClosedHandler = () =>
        {
            ResolveIfStillPending(() =>
                tcs.SetException(new EndOfStreamException()));
        };

        OnReceivedLine += onReceivedLineHandler;
        OnStreamClosed += onStreamClosedHandler;

        return tcs.Task;
    }

    private async Task Run()
    {
        var buf = new char[8 * 1024];

        while (true)
        {
            var chunkLength = await _streamReader.ReadAsync(buf, 0, buf.Length);

            if (chunkLength == 0)
            {
                OnClosed();

                break;
            }

            OnChunk(new ArraySegment<char>(buf, 0, chunkLength));

            int lineBreakPos;
            var startPos = 0;

            while ((lineBreakPos = Array.IndexOf(buf, '\n', startPos, chunkLength - startPos)) >= 0 &&
                startPos < chunkLength)
            {
                var length = lineBreakPos - startPos;

                _linesBuffer.Append(buf, startPos, length);
                OnCompleteLine(_linesBuffer.ToString());
                _linesBuffer.Clear();

                startPos = lineBreakPos + 1;
            }

            if (lineBreakPos < 0 &&
                startPos < chunkLength)
            {
                _linesBuffer.Append(buf, startPos, chunkLength - startPos);
            }
        }
    }

    protected virtual void OnChunk(ArraySegment<char> chunk)
    {
        OnReceivedChunk?.Invoke(chunk);
    }

    protected virtual void OnCompleteLine(string line)
    {
        OnReceivedLine?.Invoke(line);
    }

    protected virtual void OnClosed()
    {
        OnStreamClosed?.Invoke();
    }
}