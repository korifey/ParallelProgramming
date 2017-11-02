using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using NUnit.Framework;
using ParallelProgramming.Util;

namespace ParallelProgramming.Demo
{
    public class Demo7_Parallel
    {
        const int Lim = 3_000_000;
        private readonly IEnumerable<int> range = Enumerable.Range(0, Lim);
        
        [Test]
        public void TestNotParallel()
        {
            var res = range.Count(i => i.IsPrime());
            Console.WriteLine($"Result: {res}");
        }
        
        
        
        [Test]
        public void TestParallelFor()
        {
            int res = 0;
            Parallel.ForEach(range, i =>
            {
                if (i.IsPrime()) Interlocked.Increment(ref res);
            });
            
            Console.WriteLine($"Result: {res}");
        }

        


        [Test]
        public void TestParallelLinq()
        {
            var res = range.AsParallel().Count(i => i.IsPrime());
            Console.WriteLine($"Result: {res}");
        }
    }
}