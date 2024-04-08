using Rayer.Core.Abstractions;
using System.ComponentModel;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Input;
using static Rayer.Core.PInvoke.Win32;

namespace Rayer.Core;

public partial class ProcessMessageWindow : Window
{
    private int _hookId;
    private KeyboardHookProc? KeyboardHookDelegate;

    private readonly IPlaybarService _playbarService;
    private readonly ISettingsService _settingsService;

    public ProcessMessageWindow(IPlaybarService playbarService, ISettingsService settingsService)
    {
        _playbarService = playbarService;
        _settingsService = settingsService;

        InitializeComponent();

        WindowStartupLocation = WindowStartupLocation.Manual;
        Left = -99999;
    }

    private void OnSourceInitialized(object sender, EventArgs e)
    {
        var hInstance = Marshal.GetHINSTANCE(Assembly.GetExecutingAssembly().GetModules()[0]);

        KeyboardHookDelegate = new KeyboardHookProc(KeyboardHookProcHandler);

        _hookId = SetWindowsHookEx(13, KeyboardHookDelegate, hInstance, 0);
    }

    private void OnClosing(object sender, CancelEventArgs e)
    {
        UnhookWindowsHookEx(_hookId);
    }

    private int KeyboardHookProcHandler(int nCode, int wParam, IntPtr lParam)
    {
        if (!Application.Current.MainWindow.IsActive)
        {
            return 0;
        }

        if (nCode < 0)
        {
            return CallNextHookEx(_hookId, nCode, wParam, lParam);
        }

        if (wParam == 0x101)
        {
            return CallNextHookEx(_hookId, nCode, wParam, lParam);
        }

        if (Marshal.PtrToStructure(lParam, typeof(KBDLLHOOKSTRUCT)) is not KBDLLHOOKSTRUCT keyData)
        {
            return CallNextHookEx(_hookId, nCode, wParam, lParam);
        }

        Task.Run(async () =>
        {
            await Application.Current.Dispatcher.InvokeAsync(async () =>
            {
                await CheckKeyBindings(keyData.vkCode);
            });
        });

        return CallNextHookEx(_hookId, nCode, wParam, lParam);
    }

    private async Task CheckKeyBindings(int vkCode)
    {
        var key = KeyInterop.KeyFromVirtualKey(vkCode);

        if (key == _settingsService.Settings.KeyPlayOrPause.Key)
        {
            _playbarService.PlayOrPause();
        }

        if (key == _settingsService.Settings.KeyPrevious.Key)
        {
            await _playbarService.Previous();
        }

        if (key == _settingsService.Settings.KeyNext.Key)
        {
            await _playbarService.Next();
        }

        if (key == _settingsService.Settings.KeyPitchUp.Key)
        {
            _playbarService.PitchUp();
        }

        if (key == _settingsService.Settings.KeyPitchDown.Key)
        {
            _playbarService.PitchDown();
        }

        if (key == _settingsService.Settings.KeyForward.Key)
        {
            _playbarService.Forward();
        }

        if (key == _settingsService.Settings.KeyRewind.Key)
        {
            _playbarService.Rewind();
        }
    }
}