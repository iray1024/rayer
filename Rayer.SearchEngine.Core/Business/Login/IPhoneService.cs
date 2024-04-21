namespace Rayer.SearchEngine.Core.Business.Login;

public interface IPhoneService
{
    Task LoginAsync(string phone, string password, CancellationToken cancellationToken = default);

    Task LoginWithCaptchaAsync(string phone, string captcha, CancellationToken cancellationToken = default);
}