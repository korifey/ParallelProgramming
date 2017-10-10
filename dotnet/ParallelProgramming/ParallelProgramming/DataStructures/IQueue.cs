namespace ParallelProgramming.DataStructures
{
    public interface IQueue<T>
    {
        bool TryEnqueue(T res);
        bool TryDequeue(out T res);
        
        void Enqueue(T res);
        T    Dequeue();
    }
}