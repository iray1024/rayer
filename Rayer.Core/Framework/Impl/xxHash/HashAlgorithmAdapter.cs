using System.Security.Cryptography;

namespace Rayer.Core.Framework.Impl.xxHash;

public sealed class HashAlgorithmAdapter(
    int hashSize,
    Action reset,
    Action<byte[], int, int> update,
    Func<byte[]> digest)
    : HashAlgorithm
{
    public override int HashSize { get; } = hashSize;

    public override byte[] Hash => digest();

    protected override void HashCore(byte[] array, int ibStart, int cbSize)
    {
        update(array, ibStart, cbSize);
    }

    protected override byte[] HashFinal()
    {
        return digest();
    }

    public override void Initialize()
    {
        reset();
    }
}