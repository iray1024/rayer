using Microsoft.Extensions.DependencyInjection;
using Rayer.Core.Common;
using Rayer.FrameworkCore.Injection;
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

        //var qrContent = $"https://music.163.com/login?codekey={_key}";
        //using var qrGenerator = new QRCodeGenerator();
        //using var qrCodeData = qrGenerator.CreateQrCode(qrContent, QRCodeGenerator.ECCLevel.Q);
        //using var qrCode = new PngByteQRCode(qrCodeData);
        //var pngBytes = qrCode.GetGraphic(6);
        //var base64String = Convert.ToBase64String(pngBytes);

        var qrCodeResult = await Searcher.GetAsync(
            LoginSelector.QrCodeCreate()
                .WithParam("key", _key)
                .WithParam("qrimg", "1")
                .Build());

        //var response = new QrCodeModel
        //{
        //    Code = 200,
        //    Data = new QrCodeModel.QrCodeDetailModel
        //    {
        //        Image = $"data:image/png;base64,{base64String}",
        //        Url = string.Empty
        //    }
        //};

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
                .WithParam("ua", "pc")
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
                .WithParam("timestamp", DateTimeOffset.UtcNow.ToUnixTimeMilliseconds().ToString())
                .Build());

        var response = keyResult.ToEntity<Models.Login.QrCode.QrCodeKeyModel>();

        return response is not null ? response : default!;
    }
}