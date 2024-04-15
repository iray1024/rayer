using NAudio.Wave;
using NAudio.Wave.SampleProviders;
using Rayer.FFmpegCore;
using System.IO;
using WaveFormat = NAudio.Wave.WaveFormat;

namespace Rayer.Core.AudioReader.Flac;

internal class FlacAudioReader : WaveStream, ISampleProvider
{
    private readonly SampleChannel _sampleChannel = null!;

    private FFmpegDecoder _ffmpegDecoder = null!;
    private readonly WaveFormat _waveFormat = null!;

    public FlacAudioReader(Stream stream)
    {
        _ffmpegDecoder = new FFmpegDecoder(stream);

        if (_ffmpegDecoder is not null)
        {
            var sampleRate = _ffmpegDecoder.WaveFormat.SampleRate;
            var bitsPerSample = _ffmpegDecoder.WaveFormat.BitsPerSample;
            var channels = _ffmpegDecoder.WaveFormat.Channels;

            _waveFormat = new WaveFormat(sampleRate, bitsPerSample, channels);
        }
    }

    public FlacAudioReader(string path)
    {
        _ffmpegDecoder = new FFmpegDecoder(path);

        if (_ffmpegDecoder is not null)
        {
            var sampleRate = _ffmpegDecoder.WaveFormat.SampleRate;
            var bitsPerSample = _ffmpegDecoder.WaveFormat.BitsPerSample;
            var channels = _ffmpegDecoder.WaveFormat.Channels;

            _waveFormat = new WaveFormat(sampleRate, bitsPerSample, channels);
        }
    }

    public override WaveFormat WaveFormat => _waveFormat;

    public override long Length
    {
        get
        {
            return _ffmpegDecoder != null ? _ffmpegDecoder.Length : 0;
        }
    }

    public override long Position
    {
        get => _ffmpegDecoder?.Position ?? 0;
        set
        {
            if (_ffmpegDecoder is not null)
            {
                _ffmpegDecoder.Position = value;
            }
        }
    }

    public override int Read(byte[] buffer, int offset, int count)
    {
        if (_ffmpegDecoder == null)
        {
            return 0;
        }

        var totalBytesRead = 0;

        while (totalBytesRead < count)
        {
            var bytesToRead = count - totalBytesRead;
            var bytesReadThisTime = _ffmpegDecoder.Read(buffer, offset + totalBytesRead, bytesToRead);

            if (bytesReadThisTime == 0)
            {
                break;
            }

            totalBytesRead += bytesReadThisTime;
        }

        return totalBytesRead;
    }

    public int Read(float[] buffer, int offset, int count)
    {
        return _sampleChannel.Read(buffer, offset, count);
    }

    protected override void Dispose(bool disposing)
    {
        if (null != _ffmpegDecoder)
        {
            _ffmpegDecoder.Dispose();
            _ffmpegDecoder = null!;
        }
    }
}