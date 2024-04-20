using CommunityToolkit.Mvvm.ComponentModel;
using Rayer.Core.Http.Abstractions;
using Rayer.SearchEngine.Business.Login.Abstractions;
using Rayer.SearchEngine.Models.Response.Netease.Login.QrCode;
using System.IO;
using System.Windows;
using System.Windows.Media.Imaging;

namespace Rayer.SearchEngine.ViewModels;

public partial class LoginViewModel : ObservableObject
{
    private readonly ILoginManager _loginManager;
    private readonly ICookieManager _cookieManager;

    [ObservableProperty]
    private BitmapImage? _qrCode;

    [ObservableProperty]
    private string _state = "加载二维码中";

    public LoginViewModel(
        ILoginManager loginManager,
        ICookieManager cookieManager)
    {
        _loginManager = loginManager;
        _cookieManager = cookieManager;
    }

    public event EventHandler? QrCodeLoaded;
    public event EventHandler? QrCodeExpired;
    public event EventHandler? LoginSucceed;

    public async Task LoadAsync()
    {
        var login = _loginManager.UseQrCode();

        var keyResponse = await login.GetQrCodeKeyAsync();

        if (keyResponse is null)
        {
            State = "二维码加载失败";

            return;
        }

        var response = await login.GetQrCodeAsync(keyResponse.Data.Unikey);

        if (response is not null)
        {
            QrCode = ProcessQrCodeResponse(response.Data.Image);

            State = "等待扫码";

            QrCodeLoaded?.Invoke(null, EventArgs.Empty);
        }

        await Application.Current.Dispatcher.InvokeAsync(async () =>
        {
            QrCodeVerify checkResult;
            while (true)
            {
                await Task.Delay(100);

                checkResult = await login.CheckAsync(keyResponse.Data.Unikey);

                if (checkResult is { Code: 803 })
                {
                    State = "授权成功";

                    _cookieManager.StoreCookie();

                    _loginManager.RaiseLoginSucceed();
                    LoginSucceed?.Invoke(null, EventArgs.Empty);
                    break;
                }
                else
                {
                    switch (checkResult.Code)
                    {
                        case 800:
                            State = "二维码过期";

                            await Task.Delay(500);

                            State = "等待刷新二维码";

                            QrCodeExpired?.Invoke(null, EventArgs.Empty);
                            return;
                        case 801:
                            State = "等待扫码";
                            break;
                        case 802:
                            State = "等待确认";
                            break;
                        case 502:
                            State = "noCookie";
                            break;
                        default:
                            break;
                    }
                }
            }
        });
    }

    private static BitmapImage ProcessQrCodeResponse(string response)
    {
        var base64 = response.Replace("data:image/png;base64,", "");

        var buffer = Convert.FromBase64String(base64);

        using var stream = new MemoryStream(buffer);

        var bitmap = new BitmapImage();

        bitmap.BeginInit();
        bitmap.CacheOption = BitmapCacheOption.OnLoad;
        bitmap.StreamSource = stream;
        bitmap.EndInit();

        return bitmap;
    }
}