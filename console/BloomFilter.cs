using System;
using System.Linq;
using System.Collections;
using System.IO;
using System.Runtime.InteropServices;

namespace console
{
    internal class BloomFilter<T>
    {
        private readonly int k;
        private readonly int[] bits;
        private readonly Func<uint, T, int> hash;


        /// <summary>
        /// 
        /// </summary>
        /// <param name="numberOfElements">number of elements</param>
        /// <param name="falsePositiveRate">desired rate for false positives</param>
        /// <param name="hash">a seeded hash function</param>
        public BloomFilter(int numberOfElements, double falsePositiveRate, Func<uint, T, int> hash)
        {
            this.hash = hash;
            var n = numberOfElements;
            var p = falsePositiveRate;
            var m = (int)Math.Ceiling(-n * Math.Log(p) / (Math.Log(2) * Math.Log(2)));
            this.k = (int)Math.Ceiling(m * Math.Log(2) / (double)n); ;
            var mbytes = ((m - 1) / 32) + 1;
            this.bits = new int[mbytes];
        }

        public void Add(T item)
        {
            for (var j = 0U; j < k; j++)
            {
                var ix = hash(j, item);
                bits[ix / 32] |= (1 << (ix % 32));
            }

            // removing a bit would be: bits[ix / 32] &= ~(1 << (index % 32));
        }

        public bool Contains(T item)
        {
            for (var j = 0U; j < k; j++)
            {
                var ix = hash(j, item);
                if ((bits[ix / 32] & (1 << (ix % 32))) == 0) // bit not set
                {
                    return false;
                }
            }
            return true;
        }

        public void Save(string path)
        {
            using (var file = File.Create(path))
            {
                var bytes = MemoryMarshal.AsBytes(bits.AsSpan());
                file.Write(bytes);
            }
        }
    }
}
