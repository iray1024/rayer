using Rayer.SearchEngine.Extensions;
using Rayer.SearchEngine.Internal.Abstractions;
using Rayer.SearchEngine.Login.Abstractions;
using Rayer.SearchEngine.Models.Response.Login.QrCode;

namespace Rayer.SearchEngine.Login.Impl;

internal class QrCodeService(IServiceProvider serviceProvider) : SearchEngineBase(serviceProvider), IQrCodeService
{
    public async Task<QrCodeKeyResponse> GetQrCodeKeyAsync(CancellationToken cancellationToken = default)
    {
        var keyResult = await Search.GetAsync(
            Login.QrCodeKey()
                .Build());

        var response = keyResult.ToEntity<QrCodeKeyResponse>();

        return response is not null ? response : default!;
    }

    public async Task<QrCodeResponse> GetQrCodeAsync(string key, CancellationToken cancellationToken = default)
    {
        var qrCodeResult = await Search.GetAsync(
            Login.QrCodeCreate()
                .WithParam("key", key)
                .WithParam("qrimg", "1")
                .Build());

        var response = qrCodeResult.ToEntity<QrCodeResponse>();

        return response is not null ? response : default!;
    }

    public async Task<QrCodeVerifyResponse> CheckAsync(string key, CancellationToken cancellationToken = default)
    {
        var result = await Search.GetAsync(
            Login.QrCodeVerify()
                .WithParam("key", key)
                .Build());

        var response = result.ToEntity<QrCodeVerifyResponse>();

        return response is not null ? response : default!;
    }
}