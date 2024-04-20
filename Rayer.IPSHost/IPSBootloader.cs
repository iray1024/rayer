#if DEBUG
using Microsoft.Extensions.Logging;
#endif
using Microsoft.Extensions.Logging;
using Rayer.Core;
using Rayer.Core.Framework;
using Rayer.Core.Framework.Injection;
using Rayer.IPSHost.Utils;
using System.IO;
using System.Text.RegularExpressions;

namespace Rayer.IPSHost;

[Inject<IIPSBootloader>]
internal partial class IPSBootloader : IIPSBootloader
{
    private static readonly string _loaderPath = Path.Combine(Constants.Paths.AppDataDir, "cloud-server.exe");
    private static readonly TimeSpan _defaultStartupTimeout = TimeSpan.FromMinutes(2);
    private static readonly Regex _runingRegex = ServerRuningRegex();

    private ILogger? _logger;
    private TimeSpan _startupTimeout;

    public bool IsServerAvaliable { get; private set; } = true;

    public int Port { get; private set; } = 3000;

    public event EventHandler? Exited;

#if DEBUG
    public Task<Uri> RunAsync(ILogger? logger = null, TimeSpan? startupTimeout = null)
#else
    public Task<Uri> RunAsync(TimeSpan? startupTimeout = null)
#endif
    {
        if (!File.Exists(_loaderPath))
        {
            throw new InvalidOperationException("未注入cloud-server，请确认 Rayer 安装完整");
        }

        startupTimeout ??= _defaultStartupTimeout;

#if DEBUG
        _logger = logger;
#endif
        _startupTimeout = startupTimeout.Value;

        var portNumber = 3000;

        if (!TcpPortFinder.VerifyPort(portNumber))
        {
            portNumber = TcpPortFinder.FindAvailablePort();
        }

        return Task.Run(async () =>
        {
#if DEBUG
            var runner = NodeServerMiddleware.Run(logger, _loaderPath, $"--port {portNumber}");
#else
            var runner = NodeServerMiddleware.Run(_loaderPath, $"--port {portNumber}");
#endif
            runner.RunnerProcess.Exited += (s, e) =>
            {
                IsServerAvaliable = false;

                Exited?.Invoke(this, EventArgs.Empty);
            };

            while (true)
            {
                try
                {
                    var match = await runner.StdOut.WaitForMatch(_runingRegex);

                    if (match.Success)
                    {
                        IsServerAvaliable = true;
                        Port = portNumber;

                        return new Uri($"http://localhost:{portNumber}");
                    }

                    await Task.Delay(100);
                }
                catch (EndOfStreamException)
                {
                    IsServerAvaliable = true;
                    Port = portNumber;

                    return new Uri($"http://localhost:{portNumber}");
                }
            }
        });
    }

    public Task<Uri> Restart()
    {
#if DEBUG
        return RunAsync(_logger, _startupTimeout);
#else
        return RunAsync(_startupTimeout);
#endif
    }

    [GeneratedRegex(@"server running")]
    private static partial Regex ServerRuningRegex();
}