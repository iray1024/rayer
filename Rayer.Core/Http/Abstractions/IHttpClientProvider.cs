using System.Net.Http;

namespace Rayer.Core.Http.Abstractions;

public interface IHttpClientProvider
{
    HttpClient HttpClient { get; }
}