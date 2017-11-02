using System;
using System.Runtime.InteropServices.ComTypes;
using System.Threading;
using System.Threading.Tasks;
using NUnit.Framework;
using ParallelProgramming.Util;
using Timer = System.Timers.Timer;

namespace ParallelProgramming.Demo
{
    public class Demo10_Tasks
    {
        const int lim = 1_000_000;
        
        
        [Test]
        public void TasksTest()
        {
            var task = new Task<int>(() =>
            {
                int i = 0;
                var res = 0;
                while (i < lim)
                {
                    res += i.IsPrime() ? 1 : 0;
                    i++;
                }
                
                return res;
            });
            
            task.Start();

            Console.WriteLine(task.Result);

        }



        
        [Test]
        public void TestIoBoundTasks()
        {
            int start = Environment.TickCount;
            MyDelay().Wait();
            Console.WriteLine(Environment.TickCount - start);
        }

        
        Task MyDelay()
        {
            var tcs = new TaskCompletionSource<bool>();
            var timer = new Timer(1000);
            timer.Start();
            timer.Elapsed += (sender, args) =>
            {
                timer.Stop();
                tcs.SetResult(true);
            };
            
            return tcs.Task;
        }



        [Test]
        public void TestParentChild()
        {
            int start = Environment.TickCount;
            
            var task = Task.Run(() =>
            {
                Console.WriteLine("Some work");
                
            });
            
            var cnt = task.ContinueWith(_ =>
            {
                Console.WriteLine("Continuation");
            });
            
            
            
            //some chain
            cnt.Wait();
            
            Console.WriteLine(Environment.TickCount - start);
        }

        
      
    }
}