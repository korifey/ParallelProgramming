using System;
using System.Threading;
using NUnit.Framework;

namespace ParallelProgramming.Demo
{
    public class Demo2_ThreadAffinity
    {
        private ThreadStart ThreadSleep => () =>
        {
            Thread.Sleep(10000);
        };

        
        
        
        

        [Test]
        public void TestThreadAbort()
        {
            
            var thread = new Thread(ThreadSleep);
            thread.Start();
            
            Thread.Sleep(200);
            thread.Abort();
            thread.Join();
        }

        
        
        
        
        
        
        
     
        
        private ThreadStart ThreadForeverYieldCatch => () =>
        {
            try
            {
                Console.WriteLine($"Try: {Elapsed}");
                while (true)
                {
                    //hardwork!
                    Thread.SpinWait(1000000);
                    Thread.Yield();
                }
            }
            catch (Exception e)
            {
                Console.WriteLine($"Catch: {Elapsed}");
                Console.WriteLine(e);
                //todo reset
            }
            Console.WriteLine($"After try: {Elapsed}");
            
        };

        
        
        
        
        

        private ThreadStart ThreadSleepInFinally => () =>
        {
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

        
        
        
        
        
        
        
        private readonly AutoResetEvent FinishedEvent = new AutoResetEvent(false);
        private ThreadStart ThreadThatWaitsInterrupt => () =>
        {
            try
            {
                Console.WriteLine($"Try: {Elapsed}");
//                while (true)
//                {
//                    Thread.Sleep(1);
//                }

                FinishedEvent.WaitOne();

            }
            catch (Exception e)
            {
                Console.WriteLine($"Catch: {Elapsed}");
                Console.WriteLine(e);
            }
            Console.WriteLine($"After try: {Elapsed}");
        };

        
        
        
        

        
        private readonly ThreadLocal<int> startTime = new ThreadLocal<int>(() => Environment.TickCount);
        
        private string Elapsed => $"{- startTime.Value + Environment.TickCount} ms";
        
        
    }
}