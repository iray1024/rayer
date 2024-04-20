#if DEBUG
using Microsoft.Extensions.Logging;
#endif
using Rayer.Core;
using Rayer.Core.Framework;
using Rayer.IPSHost.EventedStream;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;
using System.Text.RegularExpressions;

namespace Rayer.IPSHost;

internal class ScriptRunner
{
    private static readonly Regex AnsiColorRegex = new("\x001b\\[[0-9;]*m", RegexOptions.None, TimeSpan.FromSeconds(1));

    public ScriptRunner(string path, string arguments, IDictionary<string, string>? envVars = null)
    {
        if (string.IsNullOrEmpty(path))
        {
            throw new ArgumentException("bootloader路径不能为空", nameof(path));
        }

        var exeName = Path.GetFileName(path);
        var workingDirectory = Path.GetDirectoryName(path);
        var completeArguments = !string.IsNullOrEmpty(arguments)
            ? arguments
            : string.Empty;

        if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
        {
            completeArguments = $"/c {exeName} {completeArguments}";
            exeName = "cmd";
        }

        var processStartInfo = new ProcessStartInfo(exeName)
        {
            Arguments = completeArguments,
            UseShellExecute = false,
            CreateNoWindow = true,
            RedirectStandardInput = true,
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            WorkingDirectory = workingDirectory
        };

        if (envVars != null)
        {
            foreach (var keyValuePair in envVars)
            {
                processStartInfo.Environment[keyValuePair.Key] = keyValuePair.Value;
            }
        }

        RunnerProcess = LaunchNodeProcess(processStartInfo);

        RunnerProcess.Exited += (s, e) =>
        {
            var snackbar = AppCore.GetRequiredService<ISnackbarFactory>();

            snackbar.ShowSecondary(
            "Cloud Server",
            $"Cloud Server已停止",
            TimeSpan.FromSeconds(3));
        };

        StdOut = new EventedStreamReader(RunnerProcess.StandardOutput);
        StdErr = new EventedStreamReader(RunnerProcess.StandardError);
    }

    public ScriptRunner(string workingDirectory, string scriptName, IDictionary<string, string> envVars, ScriptRunnerType runner)
        : this(workingDirectory, scriptName, string.Empty, envVars, runner)
    {

    }

    public ScriptRunner(string workingDirectory, string scriptName, string arguments, IDictionary<string, string> envVars, ScriptRunnerType runner)
    {
        if (string.IsNullOrEmpty(workingDirectory))
        {
            throw new ArgumentException("源文件路径不能为空", nameof(workingDirectory));
        }

        if (string.IsNullOrEmpty(scriptName))
        {
            throw new ArgumentException("脚本不能为空", nameof(scriptName));
        }

        Runner = runner;

        var exeName = GetExeName();
        var completeArguments = !string.IsNullOrEmpty(arguments)
            ? BuildCommand(runner, scriptName, arguments)
            : BuildTopCommand(runner, scriptName);

        if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
        {
            completeArguments = $"/c {exeName} {completeArguments}";
            exeName = "cmd";
        }

        var processStartInfo = new ProcessStartInfo(exeName)
        {
            Arguments = completeArguments,
            UseShellExecute = false,
            RedirectStandardInput = true,
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            WorkingDirectory = workingDirectory
        };

        if (envVars != null)
        {
            foreach (var keyValuePair in envVars)
            {
                processStartInfo.Environment[keyValuePair.Key] = keyValuePair.Value;
            }
        }

        RunnerProcess = LaunchNodeProcess(processStartInfo);

        StdOut = new EventedStreamReader(RunnerProcess.StandardOutput);
        StdErr = new EventedStreamReader(RunnerProcess.StandardError);
    }

    public EventedStreamReader StdOut { get; }
    public EventedStreamReader StdErr { get; }
    public Process RunnerProcess { get; }
    public ScriptRunnerType Runner { get; }

    public void AttachToLogger(
#if DEBUG
        ILogger logger
#endif
        )
    {
        StdOut.OnReceivedLine += line =>
        {
            if (!string.IsNullOrWhiteSpace(line))
            {
                if (line.StartsWith("<s>"))
                {
                    line = line[3..];
                }
#if DEBUG
                if (logger == null)
                {
                    Console.Error.WriteLine(line);
                }
                else
                {                    
                    logger.LogInformation("{msg}", StripAnsiColors(line).TrimEnd('\n'));
                }
#endif
            }
        };

        StdErr.OnReceivedLine += line =>
        {
            if (!string.IsNullOrWhiteSpace(line))
            {
                if (line.StartsWith("<s>"))
                {
                    line = line[3..];
                }
#if DEBUG
                if (logger == null)
                {
                    Console.Error.WriteLine(line);
                }
                else
                {
                    logger.LogError("{msg}", StripAnsiColors(line).TrimEnd('\n'));
                }
#endif
            }
        };

        StdErr.OnReceivedChunk += chunk =>
        {
            if (chunk.Array is not null)
            {
                var containsNewline = Array.IndexOf(chunk.Array, '\n', chunk.Offset, chunk.Count) >= 0;

                if (!containsNewline)
                {
                    Console.Write(chunk.Array, chunk.Offset, chunk.Count);
                }
            }
        };
    }

    public void Kill()
    {
        try { RunnerProcess?.Kill(); } catch { }
        try { RunnerProcess?.WaitForExit(); } catch { }
    }

    private string GetExeName()
    {
        return Runner switch
        {
            ScriptRunnerType.Npm => "npm",
            ScriptRunnerType.Yarn => "yarn",
            _ => "npm",
        };
    }

    private static string BuildCommand(ScriptRunnerType runner, string scriptName, string arguments)
    {
        var command = new StringBuilder();

        if (runner is ScriptRunnerType.Npm)
        {
            command.Append("run ");
        }

        command.Append(scriptName);
        command.Append(' ');

        if (!string.IsNullOrEmpty(arguments) &&
            !string.IsNullOrWhiteSpace(arguments))
        {
            if (runner is ScriptRunnerType.Npm)
            {
                command.Append("-- ");
            }

            command.Append(arguments);
        }

        return command.ToString();
    }

    private static string BuildTopCommand(ScriptRunnerType runner, string scriptName)
    {
        var command = new StringBuilder();

        if (runner is ScriptRunnerType.Npm)
        {
            command.Append(' ');
        }

        command.Append(scriptName);

        return command.ToString();
    }

    private static string StripAnsiColors(string line)
        => AnsiColorRegex.Replace(line, string.Empty);

    private static Process LaunchNodeProcess(ProcessStartInfo startInfo)
    {
        try
        {
            var process = Process.Start(startInfo);

            if (process is not null)
            {
                process.EnableRaisingEvents = true;

                return process;
            }

            return default!;
        }
        catch (Exception ex)
        {
            var message = $"启动失败：'{startInfo.FileName}'。解决方案：\n\n"
                        + $"[1] 确保安装了 '{startInfo.FileName}' 并且正确配置在系统环境变量中\n"
                        + $"    当前Path的环境变量: {Environment.GetEnvironmentVariable("PATH")}\n"
                        + "     确保可执行文件在其中一个目录中，如果没有请更新 PATH。\n\n"
                        + "[2] 有关异常的详细信息，请参见 InnerException。";

            throw new InvalidOperationException(message, ex);
        }
    }
}