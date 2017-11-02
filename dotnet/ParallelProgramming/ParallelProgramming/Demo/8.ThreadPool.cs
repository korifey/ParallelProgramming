using System;
using System.Threading;
using NUnit.Framework;

namespace ParallelProgramming.Demo
{
    public class Demo7_ThreadPool
    {
        private volatile int count;
        
       

        ManualResetEvent evt = new ManualResetEvent(false);
        
        private void EnqueueWait()
        {
            ThreadPool.RegisterWaitForSingleObject(evt, (_, __) =>
                {
                    Console.WriteLine("Wait completed: {0} {1}", Thread.CurrentThread.Name, Thread.CurrentThread.ManagedThreadId);
                }, null,
                int.MaxValue, true);
        }
        
        
        
        [Test]
        public void TestRegisterOnWaitHandle()
        {
            Console.WriteLine();
            
            EnqueueWait();
            EnqueueWait();
            EnqueueWait();
            EnqueueWait();
            EnqueueWait();
            
            Thread.Sleep(100);
            
            Console.WriteLine("Resuming");
            evt.Set();
            
            Thread.Sleep(100);
        }

        
        
        



        [Test]
        public void TestSpawnLotsOfActivities()
        {
            ThreadPool.GetAvailableThreads(out var workers, out var ioCompletion);
            
            int lim = 20;
            CountdownEvent latch = new CountdownEvent(lim);
            for (int i = 0; i < lim; i++)
            {
                ThreadPool.QueueUserWorkItem(state =>
                {
                    var s = (int) state;
                    while (count != s) Thread.SpinWait(1000);
                    count++;
                    latch.Signal();
                    
                }, lim-i-1);
            }

            
            Console.WriteLine($"Workers: {workers}, ioCompletion: {ioCompletion}");
            while (!latch.Wait(2000))
            {
                ThreadPool.GetAvailableThreads(out workers, out ioCompletion);
                Console.WriteLine($"Workers: {workers}, ioCompletion: {ioCompletion}");
            }
            Console.WriteLine("Finished");
        }


    }
}