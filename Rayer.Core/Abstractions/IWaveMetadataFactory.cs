using Rayer.Core.Models;

namespace Rayer.Core.Abstractions;

public interface IWaveMetadataFactory
{
    Task<WaveMetadata?> CreateAsync(string filepath);
}