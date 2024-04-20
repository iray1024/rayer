using Microsoft.Extensions.Logging;
using Rayer.IPSHost.Extensions;
using Rayer.IPSHost.Utils;

namespace Rayer.IPSHost;

internal static class NodeServerMiddleware
{
    public static ScriptRunner Run(
        ILogger? logger,
        string sourcePath,
        string arguments,
        Dictionary<string, string>? env = null)
    {
        var envVars = new Dictionary<string, string>
        {
            { "BROWSER", "none" },
        };

        if (env is not null)
        {
            envVars = env.Concat(new Dictionary<string, string>
            {
                { "BROWSER", "none" },
            }).ToDictionary();
        }

        var npmScriptRunner = new ScriptRunner(sourcePath, arguments, envVars);

        if (logger is not null)
        {
            npmScriptRunner.AttachToLogger(logger);
        }

        AppDomain.CurrentDomain.DomainUnload += (s, e) => npmScriptRunner?.Kill();
        AppDomain.CurrentDomain.ProcessExit += (s, e) => npmScriptRunner?.Kill();

        return npmScriptRunner;
    }

    public static ScriptRunner RunScript(
        ILogger? logger,
        string sourcePath,
        string npmScript,
        ScriptRunnerType runner)
    {
        var envVars = new Dictionary<string, string>
            {
            { "BROWSER", "none" },
        };

        var npmScriptRunner = new ScriptRunner(sourcePath, npmScript, envVars, runner: runner);

        if (logger is not null)
        {
            npmScriptRunner.AttachToLogger(logger);
        }

        AppDomain.CurrentDomain.DomainUnload += (s, e) => npmScriptRunner?.Kill();
        AppDomain.CurrentDomain.ProcessExit += (s, e) => npmScriptRunner?.Kill();

        return npmScriptRunner;
    }

    public static async void Attach(
        ILogger? logger,
        string sourcePath,
        string scriptName,
        int port = 8080,
        bool https = false,
        ScriptRunnerType runner = ScriptRunnerType.Npm)
    {
        if (string.IsNullOrEmpty(sourcePath))
        {
            throw new ArgumentException("源文件路径不能为空", nameof(sourcePath));
        }

        if (string.IsNullOrEmpty(scriptName))
        {
            throw new ArgumentException("启动脚本不能为空", nameof(scriptName));
        }

        var portTask = StartNodeServerAsync(sourcePath, scriptName, port, runner, logger);

        var targetUriTask = portTask.ContinueWith(
            task => new UriBuilder(https ? "https" : "http", "127.0.0.1", task.Result).Uri);

        var uri = await targetUriTask.WithTimeout(
            TimeSpan.FromMinutes(2),
            $"Node.js 服务器没有在超时时间 {2} 秒内开始侦听请求 " +
                $"请检查日志输出中的错误信息。");

    }

    private static Task<int> StartNodeServerAsync(
            string sourcePath,
            string npmScriptName,
            int portNumber,
            ScriptRunnerType runner,
            ILogger? logger = null)
    {
        if (portNumber < 80)
        {
            portNumber = TcpPortFinder.FindAvailablePort();
        }

        logger?.LogInformation("正在启动 Node.js 服务，端口号： {portNumber}...", portNumber);

        var envVars = new Dictionary<string, string>
            {
                { "PORT", portNumber.ToString() },
                { "DEV_SERVER_PORT", portNumber.ToString() },
                { "BROWSER", "none" },
            };

        var npmScriptRunner = new ScriptRunner(sourcePath, npmScriptName, $"--port {portNumber:0}", envVars, runner: runner);

        if (logger is not null)
        {
            npmScriptRunner.AttachToLogger(logger);
        }

        AppDomain.CurrentDomain.DomainUnload += (s, e) => npmScriptRunner?.Kill();
        AppDomain.CurrentDomain.ProcessExit += (s, e) => npmScriptRunner?.Kill();

        return Task.FromResult(portNumber);
    }
}