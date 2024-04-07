using Rayer.Controls.Immersive;

namespace Rayer.Abstractions;

public interface IImmersivePlayerService
{
    ImmersivePlayer Player { get; }

    bool IsNowImmersive { get; }

    void SetPlayer(ImmersivePlayer element);

    Task ToggleShow();

    Task Switch();


    event EventHandler? Show;
    event EventHandler? Hidden;
}