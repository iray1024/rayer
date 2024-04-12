using Rayer.FFmpegCore.Interops;
using System.Configuration;
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
                assemblyDirectory,
                Path.Combine("FFmpeg", Path.Combine("bin",
                    Path.Combine(platform, nint.Size == 8 ? "x64" : "x86"))));

            InteropHelper.RegisterLibrariesSearchPath(path);
        }

        var ffmpegSettings =
            (FFmpegConfigurationSection)ConfigurationManager.GetSection("FFmpeg");
        if (ffmpegSettings != null)
        {
            if (!string.IsNullOrEmpty(ffmpegSettings.HttpProxy))
            {
                Environment.SetEnvironmentVariable("http_proxy", ffmpegSettings.HttpProxy);
                Environment.SetEnvironmentVariable("no_proxy", ffmpegSettings.ProxyWhitelist);
            }
            if (ffmpegSettings.LogLevel != null)
            {
                FFmpegUtils.LogLevel = ffmpegSettings.LogLevel.Value;
            }
        }

        FFmpeg.av_register_all();
        FFmpeg.avcodec_register_all();

        _ = FFmpeg.avformat_network_init();
    }

    internal static unsafe AVOutputFormat[] GetOutputFormats()
    {
        List<AVOutputFormat> formats = [];

        var format = FFmpeg.av_oformat_next(null);
        while (format != null)
        {
            formats.Add(*format);
            format = FFmpeg.av_oformat_next(format);
        }

        return formats.ToArray();
    }

    internal static unsafe AVInputFormat[] GetInputFormats()
    {
        List<AVInputFormat> formats = [];

        var format = FFmpeg.av_iformat_next(null);
        while (format != null)
        {
            formats.Add(*format);
            format = FFmpeg.av_iformat_next(format);
        }

        return formats.ToArray();
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
        return ptr == nint.Zero ? throw new OutOfMemoryException("Could not allocate memory.") : ptr;
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
        return avioContext == null ? throw new FFmpegException("Could not allocate avio-context.", "avio_alloc_context") : avioContext;
    }

    internal static unsafe AVFormatContext* AvformatAllocContext()
    {
        var formatContext = FFmpeg.avformat_alloc_context();
        return formatContext == null
            ? throw new FFmpegException("Could not allocate avformat-context.", "avformat_alloc_context")
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

        return result; //stream index
    }

    internal static unsafe AVCodec* AvCodecFindDecoder(AvCodecId codecId)
    {
        var decoder = FFmpeg.avcodec_find_decoder(codecId);
        return decoder == null
            ? throw new FFmpegException(
                string.Format("Failed to find a decoder for CodecId {0}.", codecId),
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
        return frame == null ? throw new FFmpegException("Could not allocate frame.", "av_frame_alloc") : frame;
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
        catch /*(Exception e)*///当文件在内存流中受保护或者已损坏 (不要使用九歌播放损坏的音频)
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
        return dataSize <= 0 ? throw new FFmpegException("Could not calculate data size.") : dataSize;
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
            return "No description available.";
        }

        var errorMessage = Marshal.PtrToStringAnsi(new nint(buffer), 500).Trim('\0').Trim();
#if DEBUG
        Debug.WriteLineIf(Debugger.IsAttached, errorMessage);
#endif
        return errorMessage;
    }

    internal static void SetLogLevel(LogLevel level)
    {
        FFmpeg.av_log_set_level((int)level);
    }

    internal static LogLevel GetLogLevel()
    {
        return (LogLevel)FFmpeg.av_log_get_level();
    }

    internal unsafe delegate void LogCallback(void* ptr, int level, byte* fmt, nint vl);

    internal static unsafe void SetLogCallback(LogCallback callback)
    {
        FFmpeg.av_log_set_callback(Marshal.GetFunctionPointerForDelegate(callback));
    }

    internal static unsafe LogCallback GetDefaultLogCallback()
    {
        return (ptr, level, fmt, vl) =>
        {
            FFmpeg.av_log_default_callback(ptr, level, Marshal.PtrToStringAnsi(new nint(fmt)), (sbyte*)vl);
        };
    }

    internal static unsafe string FormatLine(void* avcl, int level, string fmt, nint vl,
        ref int printPrefix)
    {
        var line = string.Empty;

        const int bufferSize = 0x400;
        var buffer = stackalloc byte[bufferSize];
        fixed (int* ppp = &printPrefix)
        {
            var result = FFmpeg.av_log_format_line2(avcl, level, fmt, (sbyte*)vl, (nint)buffer, bufferSize, ppp);
            if (result < 0)
            {
                Debug.WriteLine("av_log_format_line2 failed with " + result.ToString("x8"));
                return line;
            }

            line = Marshal.PtrToStringAnsi((nint)buffer);
            if (line != null && result > 0)
            {
                line = line[..result];
            }

            return line ?? string.Empty;
        }
    }
}