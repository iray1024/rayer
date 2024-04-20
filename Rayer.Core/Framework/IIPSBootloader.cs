using Microsoft.Extensions.Logging;

namespace Rayer.Core.Framework;

public interface IIPSBootloader
{
    bool IsServerAvaliable { get; }

    int Port { get; }

    Task<Uri> RunAsync(ILogger? logger = null, TimeSpan? startupTimeout = null);

    Task<Uri> Restart();

    void Exit();

    event EventHandler Exited;
}