using Rayer.Core.Models;

namespace Rayer.Core.Abstractions;

public interface IWaveMetadataFactory
{
    WaveMetadata? Create(string filepath);
}