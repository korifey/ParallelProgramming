using System;
using System.Diagnostics;
using System.Threading;
using NUnit.Framework;
using ParallelProgramming.Util;

namespace ParallelProgramming.Demo
{
    public class Demo1_ThreadAffinity
    {

        private static readonly ParameterizedThreadStart ThreadBody = o =>
        {
            var range = (Range) o;
            var time = Environment.TickCount;

            var res = Primes.Between(range);
            
                
            Console.WriteLine($"ThreadId: {Thread.CurrentThread.ManagedThreadId}," +
                              $"range=[{range.From},{range.ToExclusive}), " +
                              $"result = {res}, " +
                              $"time = {Environment.TickCount - time} ms");
        };
        
        
        
        
        private void Workload()
        {
            const int perThread = 1_000_000_000;
            var threadStartRange = 0;
            
            var t1 = new Thread(ThreadBody);
            var t2 = new Thread(ThreadBody) ;
            
            t1.Start(new Range(threadStartRange, perThread));
            t2.Start(new Range(threadStartRange += perThread, perThread));

            t1.Join();
            t2.Join();
            
            Console.WriteLine("Workload completed");
        }

        
        
      


        [Test]
        public void TestNoThreadAffinity()
        {
            Workload();
        }
        
        [Test]
        public void TestThreadAffinity()
        {
            Console.WriteLine(Process.GetCurrentProcess().ProcessorAffinity);
            Process.GetCurrentProcess().ProcessorAffinity = (IntPtr) 1L;
            
            Workload();
        }

        
        [Test]
        public void ThreadAbort()
        {
            var thread = new Thread(() =>
            {

                try
                {
                    Thread.Sleep(10000);
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                    throw;
                }
            });
            thread.Start();
            Thread.Sleep(100);
            thread.Abort();
            thread.Join();
        }
    }
}