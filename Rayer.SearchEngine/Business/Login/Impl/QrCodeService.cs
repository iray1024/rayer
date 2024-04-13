using Rayer.Core.Framework.Injection;
using Rayer.SearchEngine.Business.Login.Abstractions;
using Rayer.SearchEngine.Extensions;
using Rayer.SearchEngine.Internal.Abstractions;
using Rayer.SearchEngine.Models.Response.Login.QrCode;

namespace Rayer.SearchEngine.Business.Login.Impl;

[Inject<IQrCodeService>(ServiceLifetime = Microsoft.Extensions.DependencyInjection.ServiceLifetime.Scoped)]
internal class QrCodeService(IServiceProvider serviceProvider) : SearchEngineBase(serviceProvider), IQrCodeService
{
    public async Task<QrCodeKeyResponse> GetQrCodeKeyAsync(CancellationToken cancellationToken = default)
    {
        var keyResult = await Searcher.GetAsync(
            Login.QrCodeKey()
                .Build());

        var response = keyResult.ToEntity<QrCodeKeyResponse>();

        return response is not null ? response : default!;
    }

    public async Task<QrCodeResponse> GetQrCodeAsync(string key, CancellationToken cancellationToken = default)
    {
        var qrCodeResult = await Searcher.GetAsync(
            Login.QrCodeCreate()
                .WithParam("key", key)
                .WithParam("qrimg", "1")
                .Build());

        var response = qrCodeResult.ToEntity<QrCodeResponse>();

        return response is not null ? response : default!;
    }

    public async Task<QrCodeVerifyResponse> CheckAsync(string key, CancellationToken cancellationToken = default)
    {
        var result = await Searcher.GetAsync(
            Login.QrCodeVerify()
                .WithParam("key", key)
                .Build());

        var response = result.ToEntity<QrCodeVerifyResponse>();

        return response is not null ? response : default!;
    }
}