using System;
using System.Runtime.InteropServices;

namespace console
{

    /// <summary>
    /// https://en.wikipedia.org/wiki/Jenkins_hash_function
    /// </summary>
    public static class Jenkins32
    {
        public static int Hash<T>(uint seed, T data) where T : unmanaged
        {
            var span = MemoryMarshal.CreateReadOnlySpan(ref data, 1);
            var bytes = MemoryMarshal.AsBytes(span);
            return Hash(seed, bytes);
        }

        public static int Hash(uint seed, ReadOnlySpan<byte> value)
        {
            var key = MemoryMarshal.Cast<byte, uint>(value);
            var hash = seed;
            for (var i = 0; i < key.Length; i++)
            {
                hash += key[i];
                hash += hash << 10;
                hash ^= hash >> 6;
            }
            hash += hash << 3;
            hash ^= hash >> 11;
            hash += hash << 15;
            return (int)(hash & 0x7FFFFFF);
        }
    }
}
