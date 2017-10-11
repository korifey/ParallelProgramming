using System.Diagnostics.Contracts;

namespace ParallelProgramming.DataStructures.Immutable
{
    public struct ImmutableQueue<T>
    {
        private readonly ImmutableStack<T> incoming;
        private readonly ImmutableStack<T> outgoing;

        private ImmutableQueue(ImmutableStack<T> incoming, ImmutableStack<T> outgoing)
        {
            this.incoming = incoming;
            this.outgoing = outgoing;
        }

        private static void Refill(ref ImmutableStack<T> o, ref ImmutableStack<T> i)
        {
            if (!o.IsEmpty) return;
            
            while (!i.IsEmpty)
            {
                i = i.Pop(out var value);
                o = o.Push(value);
            }
        }

        
        
        #region API

                
        [Pure]
        public bool IsEmpty => incoming.IsEmpty && outgoing.IsEmpty;

        [Pure]
        public ImmutableQueue<T> Dequeue(out T res)
        {
            var i = incoming;
            var o = outgoing;
            
            Refill(ref o, ref i);

            if (o.IsEmpty)
            {
                res = default(T);
                return this;
            }
            return new ImmutableQueue<T>(i, o.Pop(out res));
        }

        [Pure]
        public ImmutableQueue<T> Enqueue(T res)
        {
            return new ImmutableQueue<T>(incoming.Push(res), outgoing);
        } 

        #endregion
    }
}