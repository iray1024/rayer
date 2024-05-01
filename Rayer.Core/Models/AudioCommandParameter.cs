using Rayer.Core.Common;

namespace Rayer.Core.Models;

public record struct AudioCommandParameter
{
    public Audio Audio { get; set; }

    public ContextMenuScope Scope { get; set; }
}