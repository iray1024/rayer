using NAudio.Wave;
using Rayer.Core.Abstractions;

namespace Rayer.Services;

internal class PlaybarService(IAudioManager audioManager) : IPlaybarService
{
    public PlaybackState PlaybackState => audioManager.Playback.PlaybackState;

    public void PlayOrPause(bool SuppressEvent = false)
    {
        var currentPlaybackState = audioManager.Playback.PlaybackState;

        if (currentPlaybackState is PlaybackState.Playing)
        {
            audioManager.Playback.Pause();
        }
        else if (currentPlaybackState is PlaybackState.Paused)
        {
            audioManager.Playback.Resume(false);
        }
        else
        {
            return;
        }

        if (!SuppressEvent)
        {
            PlayOrPauseTriggered?.Invoke(null, EventArgs.Empty);
        }
    }

    public async Task Previous()
    {
        if (audioManager.Playback.PlaybackState is not PlaybackState.Stopped)
        {
            await audioManager.Playback.Previous();
        }

        PreviousTriggered?.Invoke(null, EventArgs.Empty);
    }

    public async Task Next()
    {
        if (audioManager.Playback.PlaybackState is not PlaybackState.Stopped)
        {
            await audioManager.Playback.Next();
        }

        NextTriggered?.Invoke(null, EventArgs.Empty);
    }

    public void PitchUp()
    {
        PitchUpTriggered?.Invoke(null, EventArgs.Empty);
    }

    public void PitchDown()
    {
        PitchDownTriggered?.Invoke(null, EventArgs.Empty);
    }

    public async Task Forward()
    {
        await audioManager.Playback.Jump();
    }

    public async Task Rewind()
    {
        await audioManager.Playback.Jump(true);
    }

    public event EventHandler? PlayOrPauseTriggered;
    public event EventHandler? PreviousTriggered;
    public event EventHandler? NextTriggered;
    public event EventHandler? PitchUpTriggered;
    public event EventHandler? PitchDownTriggered;
}