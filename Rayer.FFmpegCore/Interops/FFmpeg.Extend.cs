using System.Runtime.InteropServices;

namespace Rayer.FFmpegCore.Interops;

internal unsafe partial class FFmpeg
{
    [LibraryImport(libavformat, EntryPoint = "avio_alloc_context")]
    [UnmanagedCallConv(CallConvs = new Type[] { typeof(System.Runtime.CompilerServices.CallConvCdecl) })]
    internal static partial AVIOContext* avio_alloc_context(byte* @buffer, int @buffer_size, int @write_flag, void* @opaque,
        [MarshalAs(UnmanagedType.FunctionPtr)] FFmpegCalls.AvioReadData @read_packet,
        [MarshalAs(UnmanagedType.FunctionPtr)] FFmpegCalls.AvioWriteData @write_packet,
        [MarshalAs(UnmanagedType.FunctionPtr)] FFmpegCalls.AvioSeek @seek);
}