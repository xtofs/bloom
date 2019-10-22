using System;
using System.Linq;
using System.Collections.Generic;
using System.IO;

namespace console
{
    class Program
    {
        static void Main(string[] args)
        {
            const int numberOfElements = 10_000_000;
            const double falsePositiveRate = 0.001;

            var filter = new BloomFilter<Guid>(numberOfElements, falsePositiveRate, Jenkins32.Hash);

            var set = new HashSet<Guid>();

            for (var i = 0; i < numberOfElements;)
            {
                var guid = Guid.NewGuid();
                set.Add(guid);

                filter.Add(guid);

                i += 1;
                if (i % 100_000 == 0) System.Console.Write('.');
                if (i % 1_000_000 == 0) System.Console.WriteLine();
            }
            Console.WriteLine();
            filter.Save("filter.dat");

            // Console.WriteLine(filter.Peek());

            foreach (var guid in set)
            {
                var isMember = filter.Contains(guid);
                if (!isMember && set.Contains(guid))
                    Console.WriteLine($"false negative: {guid}");
            }

            const int num_tests = 10_000;
            var num_tested = 0;
            var count = 0;
            for (var i = 0; i < num_tests; i++)
            {
                var guid = Guid.NewGuid();
                if (!set.Contains(guid))
                {
                    num_tested += 1;
                    var isMember = filter.Contains(guid);
                    if (isMember)
                    {
                        Console.WriteLine($"false positive: {guid}");
                        count++;
                    }
                }
            }
            System.Console.WriteLine("actual {0:p} expected {1:p}", count / (double)num_tested, falsePositiveRate);
            Console.WriteLine("Done!");
        }
    }
}
