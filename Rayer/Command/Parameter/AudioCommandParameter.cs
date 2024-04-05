using Rayer.Core.Common;
using Rayer.Core.Models;

namespace Rayer.Command.Parameter;

internal record struct AudioCommandParameter
{
    public Audio Audio { get; set; }

    public ContextMenuScope Scope { get; set; }
}