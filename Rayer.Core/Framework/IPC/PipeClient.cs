using System.IO;
using System.IO.Pipes;

namespace Rayer.Core.Framework.IPC;

internal class PipeClient
{
    public static void SendMessage(string channelName, IEnumerable<string> message)
    {
        var sb = new StringBuilder();

        foreach (var item in message)
        {
            sb.Append(item);
            sb.Append(PipeServer.MessageDelimiter);
        }

        sb.Remove(sb.Length - PipeServer.MessageDelimiter.Length, PipeServer.MessageDelimiter.Length);

        SendMessage(channelName, sb.ToString());
    }

    public static void SendMessage(string channelName, string message)
    {
        using var pipeClient = new NamedPipeClientStream(".", channelName, PipeDirection.Out);

        pipeClient.Connect(0);

        using var writer = new StreamWriter(pipeClient) { AutoFlush = true };

        writer.Write(message);

        writer.Close();

        pipeClient.Dispose();
    }
}