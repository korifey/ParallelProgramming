using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using NUnit.Framework;
using ParallelProgramming.DataStructures;
using ParallelProgramming.DataStructures.LockBased;
using ParallelProgramming.DataStructures.LockFree;
using ParallelProgramming.DataStructures.WaitFree;

namespace ParallelProgramming.Tests
{
    
    public abstract class ConcurrentQueueTestBase
    {
        private IQueue<long> q;

        private long acc;
        private const long Limit = 1000000;
        private const long Sum = Limit*(Limit-1)/2;
        
        private const int Repeat = 1;
        private bool sequential;
        private volatile bool settersFinished;

        private void Setter ()
        {
            ThreadName.Value = "Setter";
            long nxt;
            while ((nxt = Interlocked.Increment(ref acc)) < Limit)
            {
                q.Enqueue(nxt);                     
            }
        }

        private long Getter()
        {
            ThreadName.Value = "Getter";
            
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
            Task[] tasks = new Task[count];
            for (int i = 0; i < count; i++) tasks[i] = Task.Run((Action)Setter);
            new TaskFactory().ContinueWhenAll(tasks, _ => settersFinished = true);
        }

        private long RunGettersAndWait(int count)
        {
            var tasks = new Task<long>[count];
            for (int i = 0; i < count; i++) tasks[i] = Task.Run((Func<long>)Getter);

            return tasks.Aggregate(0L, (sum, task) => sum + task.Result);
        }


        protected abstract IQueue<long> CreateQueue();
        
        [SetUp]
        public void SetUp()
        {
            acc = 0;
            sequential = true;
            settersFinished = false;
            q = CreateQueue();
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
            Assert.AreEqual(Sum, RunGettersAndWait(1));
        }
        
        [Test, Repeat(Repeat)]
        public void TestOneSettersAndSeveralGetters()
        {
            RunSetters(1);
            Assert.AreEqual(Sum, RunGettersAndWait(2));            
        }
        
        [Test, Repeat(Repeat)]
        public void TestSeveralSettersAndSeveralGetters()
        {
            sequential = false;
            RunSetters(3);
            Assert.AreEqual(Sum, RunGettersAndWait(3));
        }
        
        [Test, Repeat(Repeat)]
        public void TestManySettersAndManyGetters()
        {
            sequential = false;
            RunSetters(10);
            Assert.AreEqual(Sum, RunGettersAndWait(10));
        }
    }

    
    
    [TestFixture]
    internal class SynchronizedQueueTest : ConcurrentQueueTestBase
    {
        protected override IQueue<long> CreateQueue()
        {
            return new SynchronizedQueue<long>(); 
        }
    }
    
    [TestFixture]
    internal class SynchronizedQueueBadTest : ConcurrentQueueTestBase
    {
        protected override IQueue<long> CreateQueue()
        {
            return new BadSynchronizedQueue<long>(); 
        }
    }
    
    [TestFixture]
    internal class LinkedLockFreeQueueTest : ConcurrentQueueTestBase
    {
        protected override IQueue<long> CreateQueue()
        {
            return new LinkedLockFreeQueue<long>(); 
        }
    }
    
    [TestFixture]
    internal class ImmutableBasedLockFreeQueueTest : ConcurrentQueueTestBase
    {
        protected override IQueue<long> CreateQueue()
        {
            return new ImmutableBasedLockFreeQueue<long>(); 
        }
    }


    [TestFixture]
    internal class WaitFreeQueueTest : ConcurrentQueueTestBase
    {
        protected override IQueue<long> CreateQueue()
        {
            return new WaitFreeQueue<long>();
        }
    }
}