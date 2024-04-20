using System.Diagnostics;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Runtime.InteropServices;
using System.Text.RegularExpressions;

namespace Rayer.IPSHost.Utils;

internal static class TcpPortFinder
{
    private const string _ssPidRegex = @"(?:^|"",|"",pid=)(\d+)";
    internal static readonly char[] _separator = [' '];

    public static int FindAvailablePort()
    {
        using var listener = new TcpListener(IPAddress.Loopback, 0);

        listener.Start();

        try
        {
            return ((IPEndPoint)listener.LocalEndpoint).Port;
        }
        finally
        {
            listener.Stop();
        }
    }

    public static bool VerifyPort(int portNumber)
    {
        var listeners = IPGlobalProperties.GetIPGlobalProperties().GetActiveTcpListeners();

        return !listeners.Any(x => x.Port == portNumber);
    }

    public static bool KillPort(ushort port, bool force = false, bool tree = true)
        => Kill(GetPortPid(port), force: force, tree: tree);

    private static bool Kill(int pid, bool force = false, bool tree = true)
    {
        if (pid == -1) { return false; }

        var args = new List<string>();
        try
        {
            if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
            {
                if (force) { args.Add("-9"); }
                args.Add(pid.ToString());

                RunProcessReturnOutput("kill", string.Join(" ", args));
            }
            else if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
            {
                if (force) { args.Add("-9"); }
                args.Add(pid.ToString());

                RunProcessReturnOutput("kill", string.Join(" ", args));
            }
            else
            {
                if (force) { args.Add("/f"); }
                if (tree) { args.Add("/T"); }
                args.Add("/PID");
                args.Add(pid.ToString());

                return RunProcessReturnOutput("taskkill", string.Join(" ", args))?.StartsWith("SUCCESS") ?? false;
            }

            return true;
        }
        catch
        {
        }

        return false;
    }

    private static int GetPortPid(ushort port)
    {
        var pidOut = -1;

        var portColumn = 1;
        var pidColumn = 4;
        var pidRegex = string.Empty;

        List<string[]> results;
        if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
        {
            results = RunProcessReturnOutputSplit("netstat", "-anv -p tcp");
            results.AddRange(RunProcessReturnOutputSplit("netstat", "-anv -p udp"));
            portColumn = 3;
            pidColumn = 8;
        }
        else if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
        {
            results = RunProcessReturnOutputSplit("ss", "-tunlp");
            portColumn = 4;
            pidColumn = 6;
            pidRegex = _ssPidRegex;
        }
        else
        {
            results = RunProcessReturnOutputSplit("netstat", "-ano");
        }

        foreach (var line in results)
        {
            if (line.Length <= portColumn || line.Length <= pidColumn)
            {
                continue;
            }

            try
            {
                var portMatch = Regex.Match(line[portColumn], $"[.:]({port})");

                if (portMatch.Success)
                {
                    var portValue = int.Parse(portMatch.Groups[1].Value);

                    if (string.IsNullOrEmpty(pidRegex))
                    {
                        pidOut = int.Parse(line[pidColumn]);

                        return pidOut;
                    }
                    else
                    {
                        var pidMatch = Regex.Match(line[pidColumn], pidRegex);
                        if (pidMatch.Success)
                        {
                            pidOut = int.Parse(pidMatch.Groups[1].Value);
                        }
                    }
                }
            }
            catch
            {

            }
        }

        return pidOut;
    }

    private static List<string[]> RunProcessReturnOutputSplit(string fileName, string arguments)
    {
        var result = RunProcessReturnOutput(fileName, arguments);
        if (result == null)
        {
            return [];
        }

        var lines = result.Split(Environment.NewLine, StringSplitOptions.RemoveEmptyEntries);
        var lineWords = new List<string[]>();

        foreach (var line in lines)
        {
            lineWords.Add(line.Split(_separator, StringSplitOptions.RemoveEmptyEntries));
        }

        return lineWords;
    }

    private static string RunProcessReturnOutput(string fileName, string arguments)
    {
        Process? process = null;
        try
        {
            var si = new ProcessStartInfo(fileName, arguments)
            {
                UseShellExecute = false,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                CreateNoWindow = true
            };

            process = Process.Start(si);

            if (process is not null)
            {
                var stdOutT = process.StandardOutput.ReadToEndAsync();
                var stdErrorT = process.StandardError.ReadToEndAsync();

                if (!process.WaitForExit(10000))
                {
                    try
                    {
                        process?.Kill();
                    }
                    catch
                    {

                    }
                }

                return Task.WaitAll([stdOutT, stdErrorT], 10000)
                    ? (stdOutT.Result + Environment.NewLine + stdErrorT.Result).Trim()
                    : string.Empty;
            }

            return string.Empty;

        }
        catch
        {
            return string.Empty;
        }
        finally
        {
            process?.Close();
        }
    }
}