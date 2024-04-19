using Rayer.FFmpegCore.Interops;
using System.Diagnostics;
using System.Reflection;
using System.Runtime.InteropServices;

namespace Rayer.FFmpegCore;

internal class FFmpegCalls
{
    [Flags]
    public enum SeekFlags
    {
        SeekSet = 0,
        SeekCur = 1,
        SeekEnd = 2,
        SeekSize = 0x10000,
        SeekForce = 0x20000
    }

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate int AvioReadData(nint opaque, nint buffer, int bufferSize);

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate int AvioWriteData(nint opaque, nint buffer, int bufferSize);

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate long AvioSeek(nint opaque, long offset, SeekFlags whence);

    static FFmpegCalls()
    {
        var platform = Environment.OSVersion.Platform switch
        {
            PlatformID.Win32NT or PlatformID.Win32S or PlatformID.Win32Windows => "windows",
            PlatformID.Unix or PlatformID.MacOSX => "unix",
            _ => throw new PlatformNotSupportedException(),
        };
        var assemblyDirectory = Path.GetDirectoryName(Assembly.GetEntryAssembly()?.Location);
        if (assemblyDirectory != null)
        {
            var path = Path.Combine(
                assemblyDirectory, "ffmpeg");

            InteropHelper.RegisterLibrariesSearchPath(path);
        }

        // 目前不需要代理配置
        //var ffmpegSettings =
        //    (FFmpegConfigurationSection)ConfigurationManager.GetSection("FFmpeg");
        //if (ffmpegSettings != null)
        //{
        //    if (!string.IsNullOrEmpty(ffmpegSettings.HttpProxy))
        //    {
        //        Environment.SetEnvironmentVariable("http_proxy", ffmpegSettings.HttpProxy);
        //        Environment.SetEnvironmentVariable("no_proxy", ffmpegSettings.ProxyWhitelist);
        //    }
        //}

        FFmpeg.av_register_all();
        FFmpeg.avcodec_register_all();

        _ = FFmpeg.avformat_network_init();
    }

    internal static unsafe List<AvCodecId> GetCodecOfCodecTag(AVCodecTag** codecTag)
    {
        List<AvCodecId> codecs = [];
        uint i = 0;
        AvCodecId codecId;
        while ((codecId = FFmpeg.av_codec_get_id(codecTag, i++)) != AvCodecId.None)
        {
            codecs.Add(codecId);
        }

        return codecs;
    }

    internal static unsafe nint AvMalloc(int bufferSize)
    {
        var buffer = FFmpeg.av_malloc((ulong)bufferSize);
        var ptr = new nint(buffer);
        return ptr == nint.Zero ? throw new OutOfMemoryException("无法分配内存。") : ptr;
    }

    internal static unsafe void AvFree(nint buffer)
    {
        FFmpeg.av_free((void*)buffer);
    }

    internal static unsafe AVIOContext* AvioAllocContext(AvioBuffer buffer, bool writeable, nint userData,
        AvioReadData readData, AvioWriteData writeData, AvioSeek seek)
    {
        var bufferPtr = (byte*)buffer.Buffer;

        var avioContext = FFmpeg.avio_alloc_context(
            bufferPtr,
            buffer.BufferSize,
            writeable ? 1 : 0,
            (void*)userData,
            readData, writeData, seek);
        return avioContext == null ? throw new FFmpegException("无法分配 avio-context.", "avio_alloc_context") : avioContext;
    }

    internal static unsafe AVFormatContext* AvformatAllocContext()
    {
        var formatContext = FFmpeg.avformat_alloc_context();
        return formatContext == null
            ? throw new FFmpegException("无法分配 avformat-context.", "avformat_alloc_context")
            : formatContext;
    }

    internal static unsafe void AvformatOpenInput(AVFormatContext** formatContext, AvioContext avioContext)
    {
        (*formatContext)->pb = (AVIOContext*)avioContext.ContextPtr;
        var result = FFmpeg.avformat_open_input(formatContext, "DUMMY-FILENAME", null, null);
        FFmpegException.Try(result, "avformat_open_input");
    }

    internal static unsafe void AvformatOpenInput(AVFormatContext** formatContext, string url)
    {
        var result = FFmpeg.avformat_open_input(formatContext, url, null, null);
        FFmpegException.Try(result, "avformat_open_input");
    }

    internal static unsafe void AvformatCloseInput(AVFormatContext** formatContext)
    {
        FFmpeg.avformat_close_input(formatContext);
    }

    internal static unsafe void AvFormatFindStreamInfo(AVFormatContext* formatContext)
    {
        var result = FFmpeg.avformat_find_stream_info(formatContext, null);
        FFmpegException.Try(result, "avformat_find_stream_info");
    }

    internal static unsafe int AvFindBestStreamInfo(AVFormatContext* formatContext)
    {
        var result = FFmpeg.av_find_best_stream(
            formatContext,
            AVMediaType.AVMEDIA_TYPE_AUDIO,
            -1, -1, null, 0);
        FFmpegException.Try(result, "av_find_best_stream");

        return result;
    }

    internal static unsafe AVCodec* AvCodecFindDecoder(AvCodecId codecId)
    {
        var decoder = FFmpeg.avcodec_find_decoder(codecId);
        return decoder == null
            ? throw new FFmpegException(
                string.Format("找不到 CodecId 的解码器 {0}.", codecId),
                "avcodec_find_decoder")
            : decoder;
    }

    internal static unsafe void AvCodecOpen(AVCodecContext* codecContext, AVCodec* codec)
    {
        var result = FFmpeg.avcodec_open2(codecContext, codec, null);
        FFmpegException.Try(result, "avcodec_open2");
    }

    internal static unsafe void AvCodecClose(AVCodecContext* codecContext)
    {
        _ = FFmpeg.avcodec_close(codecContext);
    }

    internal static unsafe AVFrame* AvFrameAlloc()
    {
        var frame = FFmpeg.av_frame_alloc();
        return frame == null ? throw new FFmpegException("无法分配帧。", "av_frame_alloc") : frame;
    }

    internal static unsafe void AvFrameFree(AVFrame* frame)
    {
        FFmpeg.av_frame_free(&frame);
    }

    internal static unsafe void InitPacket(AVPacket* packet)
    {
        FFmpeg.av_init_packet(packet);
    }

    internal static unsafe void FreePacket(AVPacket* packet)
    {
        FFmpeg.av_packet_unref(packet);
    }

    internal static unsafe bool AvReadFrame(AvFormatContext formatContext, AVPacket* packet)
    {
        try
        {
            var result = FFmpeg.av_read_frame((AVFormatContext*)formatContext.FormatPtr, packet);
            return result >= 0;
        }
        catch
        {
            return false;
        }
    }

    internal static unsafe bool AvCodecDecodeAudio4(AVCodecContext* codecContext, AVFrame* frame, AVPacket* packet,
        out int bytesConsumed)
    {
        int gotFrame;
        var result = FFmpeg.avcodec_decode_audio4(codecContext, frame, &gotFrame, packet);
        FFmpegException.Try(result, "avcodec_decode_audio4");
        bytesConsumed = result;
        return gotFrame != 0;
    }

    internal static int AvGetBytesPerSample(AVSampleFormat sampleFormat)
    {
        var dataSize = FFmpeg.av_get_bytes_per_sample(sampleFormat);
        return dataSize <= 0 ? throw new FFmpegException("无法计算数据大小。") : dataSize;
    }

    internal static unsafe int AvSamplesGetBufferSize(AVFrame* frame)
    {
        var result = FFmpeg.av_samples_get_buffer_size(null, frame->channels, frame->nb_samples,
            (AVSampleFormat)frame->format, 1);
        FFmpegException.Try(result, "av_samples_get_buffer_size");
        return result;
    }

    internal static unsafe void AvFormatSeekFile(AvFormatContext formatContext, double time)
    {
        var result = FFmpeg.avformat_seek_file((AVFormatContext*)formatContext.FormatPtr,
            formatContext.BestAudioStreamIndex, long.MinValue, (long)time, (long)time, 0);

        FFmpegException.Try(result, "avformat_seek_file");
    }

    internal static unsafe string AvStrError(int errorCode)
    {
        var buffer = stackalloc byte[500];
        var result = FFmpeg.av_strerror(errorCode, new nint(buffer), 500);
        if (result < 0)
        {
            return "没有可用的描述。";
        }

        var errorMessage = Marshal.PtrToStringAnsi(new nint(buffer), 500).Trim('\0').Trim();
#if DEBUG
        Debug.WriteLineIf(Debugger.IsAttached, errorMessage);
#endif
        return errorMessage;
    }
}