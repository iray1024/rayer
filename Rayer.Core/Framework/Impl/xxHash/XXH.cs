using System.Runtime.CompilerServices;

namespace Rayer.Core.Framework.Impl.xxHash;

internal unsafe class XXH
{
    protected XXH()
    {

    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal static uint XXH_read32(void* p)
    {
        return *(uint*)p;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal static ulong XXH_read64(void* p)
    {
        return *(ulong*)p;
    }

    [MethodImpl(MethodImplOptions.AggressiveOptimization)]
    internal static void XXH_zero(void* target, int length)
    {
        var targetP = (byte*)target;

        while (length >= sizeof(ulong))
        {
            *(ulong*)targetP = 0;
            targetP += sizeof(ulong);
            length -= sizeof(ulong);
        }

        if (length >= sizeof(uint))
        {
            *(uint*)targetP = 0;
            targetP += sizeof(uint);
            length -= sizeof(uint);
        }

        if (length >= sizeof(ushort))
        {
            *(ushort*)targetP = 0;
            targetP += sizeof(ushort);
            length -= sizeof(ushort);
        }

        if (length > 0)
        {
            *targetP = 0;
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveOptimization)]
    internal static void XXH_copy(void* target, void* source, int length)
    {
        var sourceP = (byte*)source;
        var targetP = (byte*)target;

        while (length >= sizeof(ulong))
        {
            *(ulong*)targetP = *(ulong*)sourceP;
            targetP += sizeof(ulong);
            sourceP += sizeof(ulong);
            length -= sizeof(ulong);
        }

        if (length >= sizeof(uint))
        {
            *(uint*)targetP = *(uint*)sourceP;
            targetP += sizeof(uint);
            sourceP += sizeof(uint);
            length -= sizeof(uint);
        }

        if (length >= sizeof(ushort))
        {
            *(ushort*)targetP = *(ushort*)sourceP;
            targetP += sizeof(ushort);
            sourceP += sizeof(ushort);
            length -= sizeof(ushort);
        }

        if (length > 0)
        {
            *targetP = *sourceP;
        }
    }

    internal static void Validate(byte[] bytes, int offset, int length)
    {
        if (bytes == null || offset < 0 || length < 0 || offset + length > bytes.Length)
        {
            throw new ArgumentException("无效的缓冲区边界");
        }
    }
}