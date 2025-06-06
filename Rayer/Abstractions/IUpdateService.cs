namespace Rayer.Abstractions;

internal interface IUpdateService
{
    Task<bool?> CheckUpdateAsync(CancellationToken cancellationToken = default);

    Task UpdateAsync(CancellationToken cancellationToken = default);
}