namespace Rayer.SearchEngine.Login.Abstractions;

public interface IPhoneService
{
    Task SendCaptchaAsync(string phone, CancellationToken cancellationToken = default);

    Task<bool> VerifyCaptchaAsync(string phone, string captcha, CancellationToken cancellationToken = default);

    Task LoginAsync(string phone, string password, CancellationToken cancellationToken = default);

    Task LoginWithCaptchaAsync(string phone, string captcha, CancellationToken cancellationToken = default);
}