using System;
using System.Collections.Concurrent;
using System.Threading;
using NUnit.Framework;

namespace ParallelProgramming.Demo
{
    public class Demo9_SynchronizationContext
    {

        [Test]
        public void TestDefault()
        {
            ThreadPool.QueueUserWorkItem(_ =>
            {
                Console.WriteLine("pool.ctx=" + SynchronizationContext.Current);
            });
            
            Console.WriteLine("test.ctx="+SynchronizationContext.Current);
            Thread.Sleep(100);
        }



        [Test]
        public void TestSubstituteCtx()
        {
            SynchronizationContext.SetSynchronizationContext(new SynchronizationContext());

            var ctx = SynchronizationContext.Current;
            Console.WriteLine("test.ctx="+ctx);

            Console.WriteLine("Current thread: "+Thread.CurrentThread.Name);
            
            ctx.Post(_ =>
            {
                Console.WriteLine("Post: "+Thread.CurrentThread.Name);
            }, null);
            
            ctx.Send(_ =>
            {
                Console.WriteLine("Send: "+Thread.CurrentThread.Name);
            }, null);

            Thread.Sleep(100);
        }

        
        
        


        class StaContext : SynchronizationContext
        {
            static readonly BlockingCollection<Action> Actions = new BlockingCollection<Action>();
            private readonly Thread staThread;

            public StaContext()
            {
                SetWaitNotificationRequired();
                staThread = new Thread(Body) {Name = "OurSTAThread"};
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
        
        
        [Test]
        public void TestStaContext()
        {
            var ctx = new StaContext();
            
            //Start loop
            ctx.Send(_ =>
            {
                Console.WriteLine("Start send: {0}", Thread.CurrentThread.Name);
                
                ctx.Send(__ =>
                {
                    Console.WriteLine("Inner send: {0}", Thread.CurrentThread.Name);
                }, null);
                Console.WriteLine("Finish send: {0}", Thread.CurrentThread.Name);
            }, null);
            
            Console.WriteLine("After send: {0}", Thread.CurrentThread.Name);
        }
        
        
    }
}