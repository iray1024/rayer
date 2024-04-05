using Rayer.Core.Common;
using System.Windows.Controls;

namespace Rayer.Core.Abstractions;

public interface IContextMenuFactory
{
    ContextMenu CreateContextMenu(ContextMenuScope scope);
}