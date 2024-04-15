using Rayer.FFmpegCore.Interops;
using Rayer.FFmpegCore.Modules;

namespace Rayer.FFmpegCore;

internal sealed class AvStream : IDisposable
{
    private readonly unsafe AVStream* _stream;

    public unsafe AVStream Stream => _stream is null ? default : *_stream;

    public unsafe WaveFormat GetSuggestedWaveFormat()
    {
        if (_stream == null)
        {
            throw new InvalidOperationException("No stream selected.");
        }

        int bitsPerSample;
        AudioEncoding encoding;
        switch (_stream->codec->sample_fmt)
        {
            case AVSampleFormat.AV_SAMPLE_FMT_U8:
            case AVSampleFormat.AV_SAMPLE_FMT_U8P:
                bitsPerSample = 8;
                encoding = AudioEncoding.Pcm;
                break;
            case AVSampleFormat.AV_SAMPLE_FMT_S16:
            case AVSampleFormat.AV_SAMPLE_FMT_S16P:
                bitsPerSample = 16;
                encoding = AudioEncoding.Pcm;
                break;
            case AVSampleFormat.AV_SAMPLE_FMT_S32:
            case AVSampleFormat.AV_SAMPLE_FMT_S32P:
                bitsPerSample = 32;
                encoding = AudioEncoding.Pcm;
                break;
            case AVSampleFormat.AV_SAMPLE_FMT_FLT:
            case AVSampleFormat.AV_SAMPLE_FMT_FLTP:
                bitsPerSample = 32;
                encoding = AudioEncoding.IeeeFloat;
                break;
            case AVSampleFormat.AV_SAMPLE_FMT_DBL:
            case AVSampleFormat.AV_SAMPLE_FMT_DBLP:
                bitsPerSample = 32;
                encoding = AudioEncoding.IeeeFloat;
                break;
            default:
                throw new NotSupportedException("不支持音频样本格式。");
        }

        var waveFormat = new WaveFormat(_stream->codec->sample_rate, bitsPerSample, _stream->codec->channels,
            encoding);

        return waveFormat;
    }

    public unsafe AvStream(nint stream)
    {
        if (stream == nint.Zero)
        {
            throw new ArgumentNullException(nameof(stream));
        }

        _stream = (AVStream*)stream;

        var avCodecContext = _stream->codec;
        var decoder = FFmpegCalls.AvCodecFindDecoder(avCodecContext->codec_id);

        FFmpegCalls.AvCodecOpen(avCodecContext, decoder);
    }

    public unsafe void Dispose()
    {
        GC.SuppressFinalize(this);
        if (_stream != null)
        {
            FFmpegCalls.AvCodecClose(_stream->codec);
        }
    }

    ~AvStream()
    {
        Dispose();
    }
}