using Rayer.Core.Framework.Injection;
using Rayer.Core.Framework.Settings.Abstractions;
using Rayer.Core.PlayControl.Abstractions;
using System.ComponentModel;
using System.Diagnostics;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Input;
using System.Windows.Interop;
using static Rayer.Core.PInvoke.Win32.User32;

namespace Rayer.Core;

[Inject]
public partial class ProcessMessageWindow : Window
{
    private int _hookId;
    private KeyboardHookProc? KeyboardHookDelegate;

    private readonly IPlaybarService _playbarService;
    private readonly ISettingsService _settingsService;

    private int _isProcess = 1;

    private bool IsProcess
    {
        [DebuggerStepThrough]
        get => _isProcess != 0;

        [DebuggerStepThrough]
        set
        {
            _ = Interlocked.Exchange(ref _isProcess, value ? 1 : 0);
        }
    }

    public ProcessMessageWindow(IPlaybarService playbarService, ISettingsService settingsService)
    {
        _playbarService = playbarService;
        _settingsService = settingsService;

        InitializeComponent();

        WindowStartupLocation = WindowStartupLocation.Manual;
        Left = -99999;
    }

    protected override void OnSourceInitialized(EventArgs e)
    {
        base.OnSourceInitialized(e);

        var hInstance = Marshal.GetHINSTANCE(Assembly.GetExecutingAssembly().GetModules()[0]);

        KeyboardHookDelegate = new KeyboardHookProc(KeyboardHookProcHandler);

        _hookId = SetWindowsHookEx(13, KeyboardHookDelegate, hInstance, 0);

        var source = PresentationSource.FromVisual(this) as HwndSource;
        source?.AddHook(ProgramHook);
    }

    public void ToggleProcess()
    {
        IsProcess = !IsProcess;
    }

    private void OnClosing(object sender, CancelEventArgs e)
    {
        UnhookWindowsHookEx(_hookId);
    }

    [DebuggerStepThrough]
    private int KeyboardHookProcHandler(int nCode, int wParam, IntPtr lParam)
    {
        if (!Application.Current.MainWindow.IsActive || !IsProcess)
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
        if (Keyboard.Modifiers is not ModifierKeys.None)
        {
            return;
        }

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

    private IntPtr ProgramHook(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled)
    {
        if (msg == WM_SHOWRAYER)
        {
            AppCore.MainWindow.WindowState = WindowState.Normal;
        }

        return IntPtr.Zero;
    }
}