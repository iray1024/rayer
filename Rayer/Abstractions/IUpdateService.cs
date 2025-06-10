namespace Rayer.Abstractions;

internal interface IUpdateService
{
    Task<(bool?, string?)> CheckUpdateAsync(CancellationToken cancellationToken = default);

    Task UpdateAsync(CancellationToken cancellationToken = default);
}