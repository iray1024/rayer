using System.Security.Cryptography;

namespace Rayer.Core.Framework;

public interface IXXH64
{
    const ulong EmptyHash = 17241709254077376921;

    void Reset();

    unsafe void Update(byte* bytes, int length);

    void Update(ReadOnlySpan<byte> bytes);

    void Update(byte[] bytes, int offset, int length);

    ulong Digest();

    byte[] DigestBytes();

    HashAlgorithm AsHashAlgorithm();
}