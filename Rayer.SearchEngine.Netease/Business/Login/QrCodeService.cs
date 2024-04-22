using Microsoft.Extensions.DependencyInjection;
using Rayer.Core.Common;
using Rayer.Core.Framework.Injection;
using Rayer.SearchEngine.Core.Business.Login;
using Rayer.SearchEngine.Core.Domain.Authority.Login;
using Rayer.SearchEngine.Netease.Engine;

namespace Rayer.SearchEngine.Netease.Business.Login;

[Inject<IQrCodeService>(ServiceLifetime = ServiceLifetime.Scoped, ServiceKey = SearcherType.Netease)]
internal class QrCodeService : SearchEngineBase, IQrCodeService
{
    private string _key = string.Empty;

    public async Task<QrCode> GetQrCodeAsync(CancellationToken cancellationToken = default)
    {
        var qrCodeKey = await GetQrCodeKeyAsync(cancellationToken);

        _key = qrCodeKey.Data.Unikey;

        var qrCodeResult = await Searcher.GetAsync(
            LoginSelector.QrCodeCreate()
                .WithParam("key", _key)
                .WithParam("qrimg", "1")
                .Build());

        var response = qrCodeResult.ToEntity<Models.Login.QrCode.QrCodeModel>();

        if (response is not null)
        {
            var domain = Mapper.Map<QrCode>(response);

            return domain;
        }

        return default!;
    }

    public async Task<QrCodeVerify> CheckAsync(CancellationToken cancellationToken = default)
    {
        var result = await Searcher.GetAsync(
            LoginSelector.QrCodeVerify()
                .WithParam("key", _key)
                .Build());

        var response = result.ToEntity<Models.Login.QrCode.QrCodeVerifyModel>();

        if (response is not null)
        {
            var domain = Mapper.Map<QrCodeVerify>(response);

            return domain;
        }

        return default!;
    }

    private async Task<Models.Login.QrCode.QrCodeKeyModel> GetQrCodeKeyAsync(CancellationToken cancellationToken = default)
    {
        var keyResult = await Searcher.GetAsync(
            LoginSelector.QrCodeKey()
                .Build());

        var response = keyResult.ToEntity<Models.Login.QrCode.QrCodeKeyModel>();

        return response is not null ? response : default!;
    }
}