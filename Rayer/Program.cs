using Rayer.Core;
using Rayer.Core.Framework.IPC;
using Rayer.Core.PInvoke;
using Rayer.FrameworkCore;
using System.Windows;
using Wpf.Ui;
using Wpf.Ui.Extensions;

namespace Rayer;

internal class Program
{
    private static readonly Mutex _mutex = new(false, Constants.SingleInstance.UniqueAppName);
    internal static readonly string[] _defaultShowMessage = ["--showapp", "true"];

    [STAThread]
    public static void Main()
    {
        try
        {

            if (!_mutex.WaitOne(TimeSpan.FromSeconds(1), false))
            {
                try
                {
                    var args = Environment.GetCommandLineArgs().Skip(1).ToArray();

                    PipeClient.SendMessage(Constants.SingleInstance.PipeServerName, args.Length != 0 ? args : _defaultShowMessage);
                }
                catch
                {
                    Win32.User32.PostMessage(0xffff, Win32.User32.WM_SHOWRAYER, IntPtr.Zero, IntPtr.Zero);
                }

                return;
            }
        }
        catch (AbandonedMutexException e)
        {

            System.Diagnostics.Debug.WriteLine(e.Message);

        }

        try
        {
            var server = new PipeServer(Constants.SingleInstance.PipeServerName);

            server.MessageReceived += OnPipeServerMessageReceived;
        }
        catch (Exception ex)
        {
            var dialogService = AppCore.GetService<IContentDialogService>();

            if (dialogService is not null)
            {
                Application.Current.Dispatcher.Invoke(() =>
                {
                    _ = dialogService.ShowSimpleDialogAsync(new SimpleContentDialogCreateOptions
                    {
                        Title = "Rayer",
                        Content = $"创建IPC Server失败: {ex.Message}",
                        CloseButtonText = "关闭"
                    }, AppCore.StoppingToken).Result;
                });
            }
        }

        try
        {
            var app = new App();

            app.InitializeComponent();

            app.Startup += OnAppStartup;

            app.Run();
        }
        finally
        {
            _mutex.ReleaseMutex();
        }
    }

    private static void OnPipeServerMessageReceived(object? sender, string[] msg)
    {
        if (msg[0] == "--showapp" && msg[1] == "true")
        {
            Application.Current.Dispatcher.Invoke(() =>
            {
                AppCore.MainWindow.WindowState = WindowState.Normal;
            });
        }
    }

    private static void OnAppStartup(object sender, StartupEventArgs e)
    {

    }
}