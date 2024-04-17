using System.IO;
using System.IO.Pipes;

namespace Rayer.Core.Framework.IPC;

internal class PipeServer
{
    public static string MessageDelimiter { get; } = "_mm_";

    public PipeServer(string channelName)
    {
        CreateRemoteService(channelName).ConfigureAwait(false);
    }

    public event EventHandler<string[]>? MessageReceived;

    private async Task CreateRemoteService(string channelName)
    {
        var pipeServer = new NamedPipeServerStream(channelName, PipeDirection.In);

        while (true)
        {
            try
            {
                await pipeServer.WaitForConnectionAsync().ConfigureAwait(false);

                using var reader = new StreamReader(pipeServer);

                var rawArgs = await reader.ReadToEndAsync();

                MessageReceived?.Invoke(this, rawArgs.Split(MessageDelimiter));

                pipeServer.Disconnect();

                await pipeServer.DisposeAsync();

                await Task.Delay(100);
            }
            catch (ObjectDisposedException)
            {
                pipeServer = new NamedPipeServerStream(channelName, PipeDirection.In);
            }
        }
    }
}