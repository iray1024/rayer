using System.Net.Http;

namespace Rayer.Core.Http;

public interface IHttpClientProvider
{
    HttpClient HttpClient { get; }
}