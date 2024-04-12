using System.Runtime.InteropServices;

namespace Rayer.FFmpegCore.Interops;

internal unsafe partial class FFmpeg
{
    [DllImport(libavformat, EntryPoint = "avio_alloc_context", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
    internal static extern AVIOContext* avio_alloc_context(byte* @buffer, int @buffer_size, int @write_flag, void* @opaque,
        [MarshalAs(UnmanagedType.FunctionPtr)] FFmpegCalls.AvioReadData @read_packet,
        [MarshalAs(UnmanagedType.FunctionPtr)] FFmpegCalls.AvioWriteData @write_packet,
        [MarshalAs(UnmanagedType.FunctionPtr)] FFmpegCalls.AvioSeek @seek);
}