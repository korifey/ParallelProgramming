using System;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using NUnit.Framework;

namespace ParallelProgramming.Demo
{
    public class Demo3_Locks
    {
        class Counter
        {
            private int Value = 0;

            internal int Next()
            {
                return ++Value;
            }
        }
        
        
        
        
        [Test]
        public void TestCounter() {
            
            
            var counter = new Counter();
            
            const long lim = 1_000_000;

            long Body()
            {
                var res = 0L;
                for (var i = 0; i < lim; i++)
                    res += counter.Next();

                return res;
            }
            

            
            //instead of Thread.Start
            var task1 = Task.Factory.StartNew(Body);
            var task2 = Task.Factory.StartNew(Body);

            var expected = lim * (lim * 2 + 1);
            Assert.AreEqual(expected, task1.Result + task2.Result);
        }
    }
    
    
    
    
}