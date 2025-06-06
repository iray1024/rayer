using Microsoft.Extensions.Logging;
using Rayer.Core;
using Rayer.Core.Framework;
using Rayer.FrameworkCore.Injection;
using Rayer.IPSHost.Utils;
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

    private ScriptRunner _runner = default!;

    public bool IsServerAvaliable { get; private set; } = true;

    public int Port { get; private set; } = 3000;

    public event EventHandler? Exited;

    public Task<Uri> RunAsync(ILogger? logger = null, TimeSpan? startupTimeout = null)
    {
        if (!File.Exists(_loaderPath))
        {
            IsServerAvaliable = false;
            Port = -1;

            throw new InvalidOperationException("未注入cloud-server，请确认 Rayer 安装完整");
        }

        startupTimeout ??= _defaultStartupTimeout;

        _logger = logger;
        _startupTimeout = startupTimeout.Value;

        var portNumber = 3000;

        if (!TcpPortFinder.VerifyPort(portNumber))
        {
            portNumber = TcpPortFinder.FindAvailablePort();
        }

        return Task.Run(async () =>
        {
            var runner = NodeServerMiddleware.Run(logger, _loaderPath, $"--port {portNumber}");

            runner.RunnerProcess.Exited += (s, e) =>
            {
                IsServerAvaliable = false;
                Port = -1;

                Exited?.Invoke(this, EventArgs.Empty);
            };

            _runner = runner;

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
        return RunAsync(_logger, _startupTimeout);
    }

    public void Exit()
    {
        _runner?.Kill();

        TcpPortFinder.KillPort((ushort)Port, true, true);
    }

    [GeneratedRegex(@"server running")]
    private static partial Regex ServerRuningRegex();
}