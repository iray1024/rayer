using System.Diagnostics;
using System.Runtime.InteropServices;

namespace Rayer.FFmpegCore.Modules;

[StructLayout(LayoutKind.Sequential, Pack = 2)]
public class WaveFormat : ICloneable, IEquatable<WaveFormat>
{
    private AudioEncoding _encoding;
    private short _channels;
    private int _sampleRate;
    private int _bytesPerSecond;

    private short _blockAlign;
    private short _bitsPerSample;
    private short _extraSize;

    public WaveFormat()
            : this(44100, 16, 2)
    {
    }

    public WaveFormat(int sampleRate, int bits, int channels)
            : this(sampleRate, bits, channels, AudioEncoding.Pcm)
    {
    }

    public WaveFormat(int sampleRate, int bits, int channels, AudioEncoding encoding)
            : this(sampleRate, bits, channels, encoding, 0)
    {
    }

    public WaveFormat(int sampleRate, int bits, int channels, AudioEncoding encoding, int extraSize)
    {
        ArgumentOutOfRangeException.ThrowIfLessThan(sampleRate, 1);

        ArgumentOutOfRangeException.ThrowIfNegative(bits);

        if (channels < 1)
        {
            throw new ArgumentOutOfRangeException(nameof(channels), "Number of channels has to be bigger than 0.");
        }

        _sampleRate = sampleRate;
        _bitsPerSample = (short)bits;
        _channels = (short)channels;
        _encoding = encoding;
        _extraSize = (short)extraSize;

        UpdateProperties();
    }

    public virtual int Channels
    {
        get => _channels;
        protected internal set
        {
            _channels = (short)value;
            UpdateProperties();
        }
    }

    public virtual int SampleRate
    {
        get => _sampleRate;
        protected internal set
        {
            _sampleRate = value;
            UpdateProperties();
        }
    }

    public virtual int BytesPerSecond
    {
        get => _bytesPerSecond;
        protected internal set { _bytesPerSecond = value; }
    }

    public virtual int BlockAlign
    {
        get => _blockAlign;
        protected internal set { _blockAlign = (short)value; }
    }

    public virtual int BitsPerSample
    {
        get => _bitsPerSample;
        protected internal set
        {
            _bitsPerSample = (short)value;
            UpdateProperties();
        }
    }

    public virtual int ExtraSize
    {
        get => _extraSize;
        protected internal set { _extraSize = (short)value; }
    }

    public virtual int BytesPerSample => BitsPerSample / 8;

    public virtual int BytesPerBlock => BytesPerSample * Channels;

    public virtual AudioEncoding WaveFormatTag
    {
        get => _encoding;
        protected internal set { _encoding = value; }
    }

    public long MillisecondsToBytes(double milliseconds)
    {
        var result = (long)(BytesPerSecond / 1000.0 * milliseconds);
        result -= result % BlockAlign;

        return result;
    }

    public double BytesToMilliseconds(long bytes)
    {
        bytes -= bytes % BlockAlign;
        var result = bytes / (double)BytesPerSecond * 1000.0;

        return result;
    }

    internal virtual void SetWaveFormatTagInternal(AudioEncoding waveFormatTag)
    {
        WaveFormatTag = waveFormatTag;
    }

    internal virtual void SetBitsPerSampleAndFormatProperties(int bitsPerSample)
    {
        BitsPerSample = bitsPerSample;
        UpdateProperties();
    }

    protected internal virtual void UpdateProperties()
    {
        BlockAlign = BitsPerSample / 8 * Channels;
        BytesPerSecond = BlockAlign * SampleRate;
    }

    [DebuggerStepThrough]
    private StringBuilder GetInformation()
    {
        var builder = new StringBuilder();

        builder.Append("ChannelsAvailable: " + Channels);
        builder.Append("|SampleRate: " + SampleRate);
        builder.Append("|Bps: " + BytesPerSecond);
        builder.Append("|BlockAlign: " + BlockAlign);
        builder.Append("|BitsPerSample: " + BitsPerSample);
        builder.Append("|Encoding: " + _encoding);

        return builder;
    }

    public override bool Equals(object? obj)
    {
        return Equals(obj as WaveFormat);
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(
            Channels,
            SampleRate,
            BytesPerSecond,
            BlockAlign,
            BitsPerSample,
            ExtraSize,
            WaveFormatTag);
    }

    public virtual bool Equals(WaveFormat? other)
    {
        return Channels == other?.Channels &&
               SampleRate == other?.SampleRate &&
               BytesPerSecond == other?.BytesPerSecond &&
               BlockAlign == other?.BlockAlign &&
               BitsPerSample == other?.BitsPerSample &&
               ExtraSize == other?.ExtraSize &&
               WaveFormatTag == other?.WaveFormatTag;
    }

    public override string ToString() => GetInformation().ToString();

    public virtual object Clone() => MemberwiseClone();
}