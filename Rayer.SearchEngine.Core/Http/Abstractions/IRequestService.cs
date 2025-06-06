﻿namespace Rayer.SearchEngine.Core.Http.Abstractions;

public interface IRequestService
{
    Task<string> GetAsync(string url);

    Task<string> PostAsync(string url, string param);

    Task<string> PostAsJsonAsync<T>(string url, T param);

    Task<string> PostAsFormAsync(string url, Dictionary<string, string> @params);
}