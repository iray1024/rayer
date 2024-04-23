using NAudio.Wave;

namespace Rayer.Core.Effects;

internal sealed class EffectStream(WaveStream sourceStream) : WaveStream
{
    private int _channel = 0;

    public override WaveFormat WaveFormat => sourceStream.WaveFormat;

    public override long Length => sourceStream.Length;

    public override long Position
    {
        get => sourceStream.Position;
        set => sourceStream.Position = value;
    }

    public IList<IAudioEffect> Effects { get; } = [];

    public override int Read(byte[] buffer, int offset, int count)
    {
        var read = sourceStream.Read(buffer, offset, count);

        for (var i = 0; i < read >> 2; i++)
        {
            var sample = BitConverter.ToSingle(buffer, i << 2);

            if (Effects.Count == WaveFormat.Channels)
            {
                sample = Effects[_channel].Apply(sample);

                _channel = (_channel + 1) % WaveFormat.Channels;
            }

            var bytes = BitConverter.GetBytes(sample);

            buffer[(i << 2) + 0] = bytes[0];
            buffer[(i << 2) + 1] = bytes[1];
            buffer[(i << 2) + 2] = bytes[2];
            buffer[(i << 2) + 3] = bytes[3];
        }

        return read;
    }
}