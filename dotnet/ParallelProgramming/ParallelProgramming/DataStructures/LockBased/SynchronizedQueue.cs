using System.Collections.Generic;

namespace ParallelProgramming.DataStructures.LockBased
{
    public class BadSynchronizedQueue<T> : SynchronizedQueue<T>
    {
        private int x;
        public BadSynchronizedQueue()
        {
            x = queue.GetHashCode();
        }
    }
    
    public class SynchronizedQueue<T> : QueueBase<T>
    {
        protected readonly LinkedList<T> queue = new LinkedList<T>();
        
        
        public override bool TryEnqueue(T res)
        {
            lock(queue) {
                queue.AddLast(res);
                return true;
            }
        }

        public override bool TryDequeue(out T res)
        {
            lock (queue)
            {
                if (queue.Count == 0)
                {
                    res = default(T);
                    return false;
                }
                else
                {
                    res = queue.First.Value;
                    queue.RemoveFirst();
                    return true;
                }
            }
        }
    }
}