using Rayer.SearchEngine.Core.Enums;
using System.Windows;

namespace Rayer.SearchEngine.Events;

public class SwitchSearchTypeArgs(RoutedEvent routedEvent, object source) : RoutedEventArgs(routedEvent, source)
{
    public SearchType New { get; set; }
}