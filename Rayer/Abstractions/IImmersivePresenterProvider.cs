using System.Windows.Controls;

namespace Rayer.Abstractions;

internal interface IImmersivePresenterProvider
{
    UserControl Presenter { get; }
}