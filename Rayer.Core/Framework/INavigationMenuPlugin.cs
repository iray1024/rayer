namespace Rayer.Core.Framework;

public interface INavigationMenuPlugin
{
    ICollection<object> MenuItems { get; }
}