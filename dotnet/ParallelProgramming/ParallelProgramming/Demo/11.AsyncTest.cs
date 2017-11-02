using System;
using System.Threading;
using System.Threading.Tasks;
using NUnit.Framework;

namespace ParallelProgramming.Demo
{
    public class Demo11_AsyncTest
    {
        
        public async Task<int> Work()
        {
            int res = 0;
            Console.WriteLine("Work1"+Thread.CurrentThread.ManagedThreadId);
            
            await Task.Delay(100);
            
            Console.WriteLine("Work2: "+Thread.CurrentThread.ManagedThreadId);
            res++;
            
            await Task.Delay(100);
            
            Console.WriteLine("Work2: "+Thread.CurrentThread.ManagedThreadId);
            res++;
            
            return res;
        }
        
        
        [Test]
        public void TestAsync()
        {
            Work().Wait();
        }
    }
}