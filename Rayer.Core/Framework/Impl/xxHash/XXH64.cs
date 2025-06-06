using Rayer.FrameworkCore.Injection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Security.Cryptography;

namespace Rayer.Core.Framework.Impl.xxHash;

[Inject<IXXH64>]
internal sealed unsafe class XXH64 : XXH, IXXH64
{
    private const ulong P1 = 11400714785074694791ul;
    private const ulong P2 = 14029467366897019727ul;
    private const ulong P3 = 1609587929392839161ul;
    private const ulong P4 = 9650029242287828579ul;
    private const ulong P5 = 2870177450012600261ul;

    private XXH64_state _state;

    public XXH64()
    {
        Reset();
    }

    public unsafe void Reset()
    {
        fixed (XXH64_state* stateP = &_state)
        {
            XXH64_reset(stateP, 0);
        }
    }

    public unsafe void Update(byte* bytes, int length)
    {
        fixed (XXH64_state* stateP = &_state)
        {
            XXH64_update(stateP, bytes, length);
        }
    }

    public unsafe void Update(ReadOnlySpan<byte> bytes)
    {
        fixed (byte* bytesP = bytes)
        {
            Update(bytesP, bytes.Length);
        }
    }

    public unsafe void Update(byte[] bytes, int offset, int length)
    {
        Validate(bytes, offset, length);

        fixed (byte* bytesP = bytes)
        {
            Update(bytesP + offset, length);
        }
    }

    public unsafe ulong Digest()
    {
        fixed (XXH64_state* stateP = &_state)
        {
            return XXH64_digest(stateP);
        }
    }

    public byte[] DigestBytes()
    {
        return BitConverter.GetBytes(Digest());
    }

    public HashAlgorithm AsHashAlgorithm()
    {
        return new HashAlgorithmAdapter(sizeof(uint), Reset, Update, DigestBytes);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static ulong XXH_rotl64(ulong x, int r)
    {
        return (x << r) | (x >> (64 - r));
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static ulong XXH64_round(ulong acc, ulong input)
    {
        return XXH_rotl64(acc + (input * P2), 31) * P1;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static ulong XXH64_mergeRound(ulong acc, ulong val)
    {
        return ((acc ^ XXH64_round(0, val)) * P1) + P4;
    }

    [MethodImpl(MethodImplOptions.AggressiveOptimization)]
    private static ulong XXH64_hash(void* input, int len, ulong seed)
    {
        var p = (byte*)input;
        var bEnd = p + len;
        ulong h64;

        if (len >= 32)
        {
            var limit = bEnd - 32;
            var v1 = seed + P1 + P2;
            var v2 = seed + P2;
            var v3 = seed + 0;
            var v4 = seed - P1;

            do
            {
                v1 = XXH64_round(v1, XXH_read64(p + 0));
                v2 = XXH64_round(v2, XXH_read64(p + 8));
                v3 = XXH64_round(v3, XXH_read64(p + 16));
                v4 = XXH64_round(v4, XXH_read64(p + 24));
                p += 32;
            }
            while (p <= limit);

            h64 = XXH_rotl64(v1, 1) + XXH_rotl64(v2, 7) + XXH_rotl64(v3, 12) + XXH_rotl64(v4, 18);
            h64 = XXH64_mergeRound(h64, v1);
            h64 = XXH64_mergeRound(h64, v2);
            h64 = XXH64_mergeRound(h64, v3);
            h64 = XXH64_mergeRound(h64, v4);
        }
        else
        {
            h64 = seed + P5;
        }

        h64 += (ulong)len;

        while (p + 8 <= bEnd)
        {
            h64 ^= XXH64_round(0, XXH_read64(p));
            h64 = (XXH_rotl64(h64, 27) * P1) + P4;
            p += 8;
        }

        if (p + 4 <= bEnd)
        {
            h64 ^= XXH_read32(p) * P1;
            h64 = (XXH_rotl64(h64, 23) * P2) + P3;
            p += 4;
        }

        while (p < bEnd)
        {
            h64 ^= *p * P5;
            h64 = XXH_rotl64(h64, 11) * P1;
            p++;
        }

        h64 ^= h64 >> 33;
        h64 *= P2;
        h64 ^= h64 >> 29;
        h64 *= P3;
        h64 ^= h64 >> 32;

        return h64;
    }

    private static void XXH64_reset(XXH64_state* state, ulong seed)
    {
        XXH_zero(state, sizeof(XXH64_state));
        state->v1 = seed + P1 + P2;
        state->v2 = seed + P2;
        state->v3 = seed + 0;
        state->v4 = seed - P1;
    }

    [MethodImpl(MethodImplOptions.AggressiveOptimization)]
    private static void XXH64_update(XXH64_state* state, void* input, int len)
    {
        var p = (byte*)input;
        var bEnd = p + len;

        state->total_len += (ulong)len;

        if (state->memsize + len < 32)
        {
            XXH_copy((byte*)state->mem64 + state->memsize, input, len);
            state->memsize += (uint)len;
            return;
        }

        if (state->memsize > 0)
        {
            XXH_copy((byte*)state->mem64 + state->memsize, input, (int)(32 - state->memsize));
            state->v1 = XXH64_round(state->v1, XXH_read64(state->mem64 + 0));
            state->v2 = XXH64_round(state->v2, XXH_read64(state->mem64 + 1));
            state->v3 = XXH64_round(state->v3, XXH_read64(state->mem64 + 2));
            state->v4 = XXH64_round(state->v4, XXH_read64(state->mem64 + 3));
            p += 32 - state->memsize;
            state->memsize = 0;
        }

        if (p + 32 <= bEnd)
        {
            var limit = bEnd - 32;
            var v1 = state->v1;
            var v2 = state->v2;
            var v3 = state->v3;
            var v4 = state->v4;

            do
            {
                v1 = XXH64_round(v1, XXH_read64(p + 0));
                v2 = XXH64_round(v2, XXH_read64(p + 8));
                v3 = XXH64_round(v3, XXH_read64(p + 16));
                v4 = XXH64_round(v4, XXH_read64(p + 24));
                p += 32;
            }
            while (p <= limit);

            state->v1 = v1;
            state->v2 = v2;
            state->v3 = v3;
            state->v4 = v4;
        }

        if (p < bEnd)
        {
            XXH_copy(state->mem64, p, (int)(bEnd - p));
            state->memsize = (uint)(bEnd - p);
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveOptimization)]
    private static ulong XXH64_digest(XXH64_state* state)
    {
        var p = (byte*)state->mem64;
        var bEnd = (byte*)state->mem64 + state->memsize;
        ulong h64;

        if (state->total_len >= 32)
        {
            var v1 = state->v1;
            var v2 = state->v2;
            var v3 = state->v3;
            var v4 = state->v4;

            h64 = XXH_rotl64(v1, 1) + XXH_rotl64(v2, 7) + XXH_rotl64(v3, 12) + XXH_rotl64(v4, 18);
            h64 = XXH64_mergeRound(h64, v1);
            h64 = XXH64_mergeRound(h64, v2);
            h64 = XXH64_mergeRound(h64, v3);
            h64 = XXH64_mergeRound(h64, v4);
        }
        else
        {
            h64 = state->v3 + P5;
        }

        h64 += state->total_len;

        while (p + 8 <= bEnd)
        {
            h64 ^= XXH64_round(0, XXH_read64(p));
            h64 = (XXH_rotl64(h64, 27) * P1) + P4;
            p += 8;
        }

        if (p + 4 <= bEnd)
        {
            h64 ^= XXH_read32(p) * P1;
            h64 = (XXH_rotl64(h64, 23) * P2) + P3;
            p += 4;
        }

        while (p < bEnd)
        {
            h64 ^= *p * P5;
            h64 = XXH_rotl64(h64, 11) * P1;
            p++;
        }

        h64 ^= h64 >> 33;
        h64 *= P2;
        h64 ^= h64 >> 29;
        h64 *= P3;
        h64 ^= h64 >> 32;

        return h64;
    }

    [StructLayout(LayoutKind.Sequential)]
    private struct XXH64_state
    {
        public ulong total_len;
        public ulong v1;
        public ulong v2;
        public ulong v3;
        public ulong v4;
        public fixed ulong mem64[4];
        public uint memsize;
    }
}