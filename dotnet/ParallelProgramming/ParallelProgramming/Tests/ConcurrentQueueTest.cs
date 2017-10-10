using System;
using System.Threading;
using System.Threading.Tasks;
using NUnit.Framework;
using ParallelProgramming.DataStructures;
using ParallelProgramming.DataStructures.LockFree;

namespace ParallelProgramming.Tests
{
    
    public abstract class ConcurrentQueueTestBase
    {
        private IQueue<long> q;

        private long acc;
        private const long Limit = 5000000;
        private const int Repeat = 1;
        private bool sequential;
        private volatile bool settersFinished;

        private void Setter ()
        {
            long nxt;
            while ((nxt = Interlocked.Increment(ref acc)) < Limit)
            {
                q.Enqueue(nxt);                    
            }
        }

        private long Getter()
        {
            long sum = 0;
            long prev = 0;
            while (true)
            {                    
                if (q.TryDequeue(out var cur))
                {
                    if (sequential) Assert.Less(prev, cur);
                    sum += cur;
                    prev = cur;
                } else if (settersFinished)
                {
                    break;
                }
            }

            return sum;
        }

        private void RunSetters(int count)
        {
            Task[] setters = new Task[count];
            for (int i = 0; i < count; i++) setters[i] = Task.Run((Action)Setter);
            new TaskFactory().ContinueWhenAll(setters, _ => settersFinished = true);
        }


        protected abstract IQueue<long> CreateQueue();
        
        [SetUp]
        public void SetUp()
        {
            acc = 0;
            sequential = true;
            settersFinished = false;
            q = new LockFreeQueue<long>();
        }
        
        [Test]
        public void TestOneThread()
        {
            q.Enqueue(1);
            q.Enqueue(2);

            Assert.True(q.TryDequeue(out var res) && res == 1);
            Assert.True(q.TryDequeue(out res) && res == 2);
            Assert.False(q.TryDequeue(out res));
        }
        
        
        [Test, Repeat(Repeat)]
        public void TestOneSetterAndOneGetter()
        {
            RunSetters(1);
            var getter = Task.Run((Func<long>)Getter);
            Assert.AreEqual(Limit*(Limit - 1)/2, getter.Result);
        }
        
        [Test, Repeat(Repeat)]
        public void TestOneSettersAndSeveralGetters()
        {
            RunSetters(1);

            var getter1 = Task.Run((Func<long>) Getter);
            var getter2 = Task.Run((Func<long>) Getter);
            Task.WaitAll(getter1, getter2);
            
            Assert.AreEqual(Limit*(Limit - 1)/2, getter1.Result + getter2.Result);
        }
        
        [Test, Repeat(Repeat)]
        public void TestSeveralSettersAndSeveralGetters()
        {
            sequential = false;
            RunSetters(3);

            var getter1 = Task.Run((Func<long>) Getter);
            var getter2 = Task.Run((Func<long>) Getter);
            var getter3 = Task.Run((Func<long>) Getter);
            Task.WaitAll(getter1, getter2, getter3);
            
            
            Assert.AreEqual(Limit*(Limit - 1)/2, getter1.Result + getter2.Result + getter3.Result);
        }
    }

    
    
    [TestFixture]
    internal class LockFreeQueueTest : ConcurrentQueueTestBase
    {
        protected override IQueue<long> CreateQueue()
        {
            return new LockFreeQueue<long>(); 
        }
    } 
}