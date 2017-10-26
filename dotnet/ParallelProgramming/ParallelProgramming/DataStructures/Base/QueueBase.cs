using System.Threading;
using ParallelProgramming.DataStructures;

namespace ParallelProgramming
{
    public abstract class QueueBase<T> : IQueue<T>
    {
        public abstract bool TryEnqueue(T res);
        public abstract bool TryDequeue(out T res);

        public void Enqueue(T res)
        {
            while (!TryEnqueue(res))
            {
                Thread.Yield();
            }
        }

        public T Dequeue()
        {
            T res;
            while (!TryDequeue(out res))
            {
                Thread.Yield();
            }
            return res;
        }
    }
}