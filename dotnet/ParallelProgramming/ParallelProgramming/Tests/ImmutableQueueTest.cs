using NUnit.Framework;
using ParallelProgramming.DataStructures.Immutable;

namespace ParallelProgramming.Tests
{
    [TestFixture]
    public class ImmutableQueueTest
    {
        
        [Test]
        public void TestBasic()
        {
            var empty = new ImmutableQueue<long>();
            Assert.True(empty.IsEmpty);
            var q1 = empty.Enqueue(1);
            
            Assert.False(q1.IsEmpty);

            var empty1 = q1.Dequeue(out var x1);
            Assert.AreEqual(1, x1);
            
            q1.Dequeue(out x1);
            Assert.AreEqual(1, x1);
            Assert.True(empty1.IsEmpty);
            
            
            
            var q2 = q1.Enqueue(2);
            Assert.False(q2.IsEmpty);
            
            q2.Dequeue(out x1);
            Assert.AreEqual(1, x1);

            var q3 = q2.Dequeue(out x1);
            Assert.AreEqual(1, x1);
            var q4 = q3.Dequeue(out var x2);
            Assert.AreEqual(2, x2);            
        }
    }
}