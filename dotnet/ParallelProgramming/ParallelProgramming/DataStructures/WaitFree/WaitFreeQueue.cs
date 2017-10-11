
using System;
using ParallelProgramming.DataStructures.Immutable;

namespace ParallelProgramming.DataStructures.WaitFree
{
    public class WaitFreeQueue<T> : QueueBase<T>
    {
        private readonly WaitFreeCombinator<ImmutableQueue<T>> combinator = new WaitFreeCombinator<ImmutableQueue<T>>(new ImmutableQueue<T>());

        private static readonly WaitFreeCombinator<ImmutableQueue<T>>.ApplyDelegate EnqDelegate =
            (ImmutableQueue<T> data, object req, out object result) =>
            {
                result = true;
                return data.Enqueue((T) req);
            };
        
        private static readonly WaitFreeCombinator<ImmutableQueue<T>>.ApplyDelegate DeqDelegate =
            (ImmutableQueue<T> data, object req, out object result) =>
            {
                if (data.IsEmpty)
                {
                    result = Tuple.Create(false, default(T));
                    return data;
                }
                else
                {
                    T res;
                    var newData = data.Dequeue(out res);
                    result = Tuple.Create(true, res);
                    return newData;
                }
                
            };                    
            
        public override bool TryEnqueue(T res)
        {
            return (bool) combinator.Apply(EnqDelegate, res);
        }

        public override bool TryDequeue(out T res)
        {
            var t = (Tuple<bool, T>)combinator.Apply(DeqDelegate, null);
            res = t.Item2;
            return t.Item1;
        }
    }
}