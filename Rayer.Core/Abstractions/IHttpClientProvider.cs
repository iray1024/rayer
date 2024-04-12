using System.Net.Http;

namespace Rayer.Core.Abstractions;

public interface IHttpClientProvider
{
    HttpClient HttpClient { get; }
}