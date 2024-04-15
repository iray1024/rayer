using NAudio.Wave;
using Rayer.Core.AudioReader.Flac;
using System.Diagnostics;

namespace Rayer.IntegrationTest;

public class FlacAudioReaderTest
{
    private static readonly string filepath = @"C:\Users\mm\Music\王樾安 - 会开花的云.flac";

    [Fact]
    public async Task FlacAudioReaderCoverPercent()
    {
        var waveStream = new FlacAudioReader(filepath);

        var device = new WaveOutEvent() { Volume = 1.0f, DesiredLatency = 200 };

        device.Init(waveStream);

        device.Play();

        var watch = new Stopwatch();
        watch.Start();

        while (device.PlaybackState != PlaybackState.Stopped)
        {
            await Task.Delay(100);

            if (watch.ElapsedMilliseconds > 5000)
            {
                break;
            }
        }
    }
}