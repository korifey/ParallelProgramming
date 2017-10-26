using System.Collections.Generic;

namespace ParallelProgramming.DataStructures.LockBased
{

    
    public class SynchronizedQueue<T> : QueueBase<T>
    {
        protected readonly LinkedList<T> Queue = new LinkedList<T>();
        
        
        public override bool TryEnqueue(T res)
        {
            lock(Queue) {
                Queue.AddLast(res);
                return true;
            }
        } 

        public override bool TryDequeue(out T res)
        {
            lock (Queue)
            {
                if (Queue.Count == 0)
                {
                    res = default(T);
                    return false;
                }
                else
                {
                    res = Queue.First.Value;
                    Queue.RemoveFirst();
                    return true;
                }
            }
        }
    }
}