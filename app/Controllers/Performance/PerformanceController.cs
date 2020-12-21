using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime;
using System.Threading;

using Microsoft.AspNetCore.Mvc;

namespace AspNetBox.Controllers
{
    [Produces("text/plain")]
    [Route("[controller]")]
    public class PerformanceController : ControllerBase
    {
        /// <summary>
        /// Allocates a temporary memory with bytes
        /// </summary>
        /// <param name="sizeMb">Memory allocation size</param>
        /// <param name="secondsBeforeFree">Seconds before releasing the allocated memory</param>
        /// <param name="cleanGC">When true, runs GC.Collect before exiting</param>
        /// <returns>Memory allocation info</returns>
        [HttpGet("MemoryAllocate")]
        public string MemoryAllocate(int sizeMb = 1000, int secondsBeforeFree = 5, bool cleanGC = false)
        {
            var msg = $"before execution - GC.GetTotalMemory(false) in mega bytes: {GC.GetTotalMemory(false) / (1024 * 1024)}";

            var wastedMemory = new List<byte[]>();
            var rnd = new Random();
            while (wastedMemory.Count < sizeMb)
            {
                var buffer = new byte[1024 * 1024]; // Allocate 1mb
                rnd.NextBytes(buffer);
                wastedMemory.Add(buffer);
            };

            msg += $"\nafter execution - GC.GetTotalMemory(false) in mega bytes: {GC.GetTotalMemory(false) / (1024 * 1024)}";

            Thread.Sleep(secondsBeforeFree * 1000);

            msg += $"\nAllocated {wastedMemory.Count} mega bytes sucessfully";

            wastedMemory = null;

            if (cleanGC)
            {
                RunGC();

                msg += $"\nGC Collect";
                msg += $"\nafter collect - GC.GetTotalMemory(true) in mega bytes: {GC.GetTotalMemory(true) / (1024 * 1024)}";
            }

            return msg;
        }

        /// <summary>
        /// Runs GC.Collect
        /// </summary>
        /// <returns></returns>
        [HttpGet("MemoryCollect")]
        public string MemoryCollect()
        {
            var before = $"before collect - GC.GetTotalMemory(false): {GC.GetTotalMemory(false) / (1024 * 1024)}";

            RunGC();

            var after = $"after collect - GC.GetTotalMemory(true): {GC.GetTotalMemory(true) / (1024 * 1024)}";
            return $"{before} \n{after}";
        }

        /// <summary>
        /// Benchmarks CPU compute speed by retreiving prime numbers
        /// </summary>
        /// <param name="primeCount">The nth primes to be retreived</param>
        /// <param name="threads">The number of threads to be used</param>
        /// <returns>Benchmark results</returns>
        [HttpGet("Cpu")]
        public string Cpu(int primeCount = 1000, int threads = 1)
        {
            var st = Stopwatch.StartNew();
            var threadArr = new List<Thread>();

            for (int i = 0; i < threads; i++)
            {
                var thread = new Thread((object? num) =>
                {
                    var threadId = Convert.ToInt32(num);
                    var computeResults = new List<long>();
                    var counter = 0;
                    while (counter++ < primeCount)
                    {
                        if (counter % threads != threadId)
                            continue;

                        computeResults.Add(FindPrimeNumber(counter));
                    }
                });

                threadArr.Add(thread);
                thread.Start(i);
            }

            foreach (var thread in threadArr)
            {
                thread.Join();
            }

            return $"{primeCount} primes computed in {st.ElapsedMilliseconds} ms (threads: {threads})";
        }

        private static void RunGC()
        {
            GCSettings.LargeObjectHeapCompactionMode = GCLargeObjectHeapCompactionMode.CompactOnce;
            GC.Collect();
            GC.WaitForPendingFinalizers();
        }

        private static long FindPrimeNumber(int n)
        {
            int count = 0;
            long a = 2;
            while (count < n)
            {
                long b = 2;
                int prime = 1;
                while (b * b <= a)
                {
                    if (a % b == 0)
                    {
                        prime = 0;
                        break;
                    }
                    b++;
                }
                if (prime > 0)
                {
                    count++;
                }
                a++;
            }
            return (--a);
        }
    }
}
