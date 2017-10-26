using System;
using System.Diagnostics;
using System.Runtime.InteropServices.ComTypes;
using System.Threading;
using NUnit.Framework;
using ParallelProgramming.Util;

namespace ParallelProgramming.Demo
{
    public class Demo2_ThreadAffinity
    {
        private ThreadStart ThreadSleep => () =>
        {
            Thread.Sleep(10000);
        };


        [Test]
        public void ThreadAbort()
        {
            
            var thread = new Thread(ThreadSleepCatch);
            thread.Start();
            
            Thread.Sleep(100);
            thread.Abort();
            thread.Join();
        }

     
        
        private ThreadStart ThreadSleepCatch => () =>
        {
            try
            {
                Console.WriteLine($"Try: {Elapsed}");
                Thread.Sleep(10000);
            }
            catch (Exception e)
            {
                Console.WriteLine($"Catch: {Elapsed}");
                Console.WriteLine(e);
            }
            Console.WriteLine($"After try: {Elapsed}");
            
        };

        
        
        

        private ThreadStart ThreadSleepInFinally => () =>
        {
            var start = Environment.TickCount;
            try
            {
                Console.WriteLine($"Try: {Elapsed}");
            }
            finally
            {
                Console.WriteLine($"Started finally: {Elapsed}");
                Thread.Sleep(2000);
                Console.WriteLine($"Finished finally: {Elapsed}");
            }
        };


        
        private readonly ThreadLocal<int> startTime = new ThreadLocal<int>(() => Environment.TickCount);
        
        private int Elapsed => - startTime.Value + Environment.TickCount;
        
        
    }
}