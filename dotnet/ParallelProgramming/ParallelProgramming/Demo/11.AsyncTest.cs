using System;
using System.Collections.Concurrent;
using System.Runtime.CompilerServices;
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
            Console.WriteLine("Work1: "+Thread.CurrentThread.ManagedThreadId);

            await Foo();
            
            Console.WriteLine("Work2: "+Thread.CurrentThread.ManagedThreadId);


            try
            {
                var task = Task.Delay(100).ConfigureAwait(false);
                //chtoto
                await task;

                Console.WriteLine("Work3: " + Thread.CurrentThread.Name);
                res++;
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
            finally
            {
                
            }

            await Task.Delay(100);
            
            Console.WriteLine("Work4: "+Thread.CurrentThread.Name);
            res++;
            
            return res;
        }

        private async Task Foo()
        {
            Console.WriteLine("Foo: " + Thread.CurrentThread.ManagedThreadId);
        }
        
        private Task Boo()
        {
            return Task.Run(() =>
            {
                throw new Exception("Hi");
            });
        }


        [Test]
        public void TestAsync()
        {
            Console.WriteLine("Main: "+Thread.CurrentThread.ManagedThreadId);
            SynchronizationContext.SetSynchronizationContext(new StaContext());
            Work().Wait();
        }
    }
    
    
    public class StaContext : SynchronizationContext
    {
        static readonly BlockingCollection<Action> Actions = new BlockingCollection<Action>();
        private readonly Thread staThread;

        public StaContext()
        {
            SetWaitNotificationRequired();
            staThread = new Thread(Body) {Name = "OurSTAThread", IsBackground = true};
            staThread.Start();
        }

        private void Body()
        {
            SetSynchronizationContext(this);
            while (true)
            {
                var action = Actions.Take();
                action();
            }
        }

        public override void Post(SendOrPostCallback d, object state)
        {
            Actions.Add( () =>
            {
                d(state);
            });
        }

        public override void Send(SendOrPostCallback d, object state)
        {
            var evt = new ManualResetEvent(false);
            Actions.Add( () =>
            {
                d(state);
                evt.Set();
            });
            evt.WaitOne();
        }


        public override int Wait(IntPtr[] waitHandles, bool waitAll, int millisecondsTimeout)
        {
            Pump();
            return WaitHelper(waitHandles, waitAll, millisecondsTimeout);
        }

        private void Pump()
        {
            while (Actions.Count > 0)
            {
                var action = Actions.Take();
                action();
            }
        }

    }
}