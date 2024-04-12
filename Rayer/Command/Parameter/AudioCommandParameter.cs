using Rayer.Core.Common;

namespace Rayer.Command.Parameter;

internal record struct AudioCommandParameter
{
    public Audio Audio { get; set; }

    public ContextMenuScope Scope { get; set; }
}