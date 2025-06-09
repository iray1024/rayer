using Rayer.Core.Common;
using System.Windows.Controls;

namespace Rayer.Core.Menu;

public interface IContextMenuFactory
{
    ContextMenu CreateContextMenu(ContextMenuScope scope, object? commandParameter = null);
}