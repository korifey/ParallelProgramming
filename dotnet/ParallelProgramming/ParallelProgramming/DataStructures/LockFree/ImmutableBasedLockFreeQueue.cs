
using System.Threading;
using ParallelProgramming.DataStructures.Immutable;

namespace ParallelProgramming.DataStructures.LockFree
{
    public class ImmutableBasedLockFreeQueue<T> : QueueBase<T>
    {
        
        Boxed<ImmutableQueue<T>> queue = Boxed.Of(new ImmutableQueue<T>());
        
        
        public override bool TryEnqueue(T res)
        {
            while (true)
            {
                var q = queue;
                var @new = Boxed.Of(q.Value.Enqueue(res));
                if (Interlocked.CompareExchange(ref queue, @new, q) == q) return true;
            }            
        }

        public override bool TryDequeue(out T res)
        {
            while (true)
            {
                var q = queue;
                if (q.Value.IsEmpty)
                {
                    res = default(T);
                    return false;
                }
                var @new = Boxed.Of(q.Value.Dequeue(out res));
                if (Interlocked.CompareExchange(ref queue, @new, q) == q) return true;
            }  
        }
    }
}