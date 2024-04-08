using Rayer.Core.Models;

namespace Rayer.Core.Events;

public class AudioChangedArgs : EventArgs
{
    public Audio New { get; set; } = null!;
}