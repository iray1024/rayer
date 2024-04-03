using NAudio.Wave;

namespace Rayer.Core.Abstractions;

public interface IPlaybarService
{
    PlaybackState PlaybackState { get; }

    void PlayOrPause(bool SuppressEvent = false);

    Task Previous();

    Task Next();

    void PitchUp();

    void PitchDown();

    void Forward();

    void Rewind();

    event EventHandler? PlayOrPauseTriggered;
    event EventHandler? PreviousTriggered;
    event EventHandler? NextTriggered;
    event EventHandler? PitchUpTriggered;
    event EventHandler? PitchDownTriggered;
}