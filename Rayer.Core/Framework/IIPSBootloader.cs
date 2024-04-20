#if DEBUG
using Microsoft.Extensions.Logging;
#endif

namespace Rayer.Core.Framework;

public interface IIPSBootloader
{
    bool IsServerAvaliable { get; }

    int Port { get; }

#if DEBUG
    Task<Uri> RunAsync(ILogger? logger = null, TimeSpan? startupTimeout = null);
#else
    Task<Uri> RunAsync(TimeSpan? startupTimeout = null);
#endif
    Task<Uri> Restart();

    event EventHandler Exited;
}