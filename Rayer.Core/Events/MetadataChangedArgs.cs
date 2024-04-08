using Rayer.Core.Models;

namespace Rayer.Core.Events;

public class MetadataChangedArgs(WaveMetadata @new) : EventArgs
{
    public WaveMetadata New = @new;
}