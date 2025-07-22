using CommunityToolkit.Mvvm.ComponentModel;
using Rayer.Core.Http;
using Rayer.SearchEngine.Core.Business.Login;
using Rayer.SearchEngine.Core.Domain.Authority.Login;
using System.IO;
using System.Windows;
using System.Windows.Media.Imaging;

namespace Rayer.SearchEngine.ViewModels;

public partial class LoginViewModel(
    ILoginManager loginManager,
    ICookieManager cookieManager) : ObservableObject
{
    [ObservableProperty]
    private BitmapImage? _qrCode;

    [ObservableProperty]
    private string? _nickName;

    [ObservableProperty]
    private string? _avatarUrl;

    [ObservableProperty]
    private string _state = "加载二维码中";

    public event EventHandler? QrCodeLoaded;
    public event EventHandler? QrCodeExpired;
    public event EventHandler? LoginSucceed;

    public async Task LoadAsync()
    {
        var login = loginManager.UseQrCode();

        var response = await login.GetQrCodeAsync();

        if (response is not null)
        {
            QrCode = ProcessQrCodeResponse(response.Image);

            State = "等待扫码";

            QrCodeLoaded?.Invoke(null, EventArgs.Empty);
        }

        await Application.Current.Dispatcher.InvokeAsync(async () =>
        {
            QrCodeVerify checkResult;
            while (true)
            {
                await Task.Delay(3000);

                checkResult = await login.CheckAsync();

#if DEBUG
                Console.WriteLine($"code={checkResult.Code}, message={checkResult.Message}, cookie={checkResult.Cookie}");
#endif
                if (checkResult is { Code: 803 })
                {
                    State = "授权成功";

                    cookieManager.StoreCookie();

                    loginManager.RaiseLoginSucceed();
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
                            NickName = checkResult.NickName;
                            AvatarUrl = checkResult.AvatarUrl;
                            State = "授权中";
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