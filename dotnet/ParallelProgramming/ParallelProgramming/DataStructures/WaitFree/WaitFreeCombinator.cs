using System;
using System.Collections.Immutable;
using System.Runtime.CompilerServices;
using System.Threading;

namespace ParallelProgramming.DataStructures.WaitFree
{
    public class WaitFreeCombinator<T>
    {
        public delegate T ApplyDelegate(T data, object param, out object result);    
        
        class TlRequest
        {            
            internal long Version;
            internal object Parameter;
            internal ApplyDelegate Op;
            internal int ThreadId;

            public void Put(ApplyDelegate op, object parameter)
            {
                Op = op;
                Parameter = parameter;
                var v = Interlocked.Increment(ref Version);
                ThreadId = Thread.CurrentThread.ManagedThreadId;
            }
        }

        class TlResult
        {
            internal readonly long Version;
            internal readonly object Result;

            public TlResult(long version, object result)
            {
                Version = version;
                Result = result;
            }
        }

        private class State
        {
            internal readonly T Data;
            internal readonly ImmutableDictionary<int, TlResult> Board;            

            public State(T data, ImmutableDictionary<int, TlResult> board)
            {
                Data = data;
                Board = board;
            }
        }
        
        // ReSharper disable once ArgumentsStyleLiteral
        private readonly ThreadLocal<TlRequest> requests = new ThreadLocal<TlRequest>(() => new TlRequest(), trackAllValues: true);
        private volatile State state;


        public WaitFreeCombinator(T initialData)
        {
            state = new State(initialData, ImmutableDictionary<int, TlResult>.Empty);
        }

        public object Apply(ApplyDelegate op, object parameter)
        {
            requests.Value.Put(op, parameter);
            Interlocked.MemoryBarrier();
            Eval();
            Interlocked.MemoryBarrier();
            var threadId = Thread.CurrentThread.ManagedThreadId;
            return state.Board[threadId].Result;
        }

        
        private void Eval()
        {
            for (int attempt = 0; attempt < 2; attempt++)
            {
                var s = state;
                var d = s.Data;
                var board = s.Board;
                
                foreach (var request in requests.Values)
                {                    
                    if (!board.TryGetValue(request.ThreadId, out var tlResult)) tlResult = new TlResult(0, null);
//                    Console.WriteLine("op="+ThreadName.Value+", threadId="+request.ThreadId+", tlResult.version="+tlResult.Version+", request.version="+request.Version);
                    if (request.Version == tlResult.Version + 1)
                    {
                        d = request.Op(d, request.Parameter, out var res);
                        board = board.SetItem(request.ThreadId, new TlResult(tlResult.Version + 1, res));
                    }
                }

                if (Interlocked.CompareExchange(ref state, new State(d, board), s) == s)
                {
//                    Console.WriteLine("Success: "+ThreadName.Value+" ->"+requests.Value.Version);
                    return;
                }
            }

//            Console.WriteLine("Failed: "+ThreadName.Value+" ->"+requests.Value.Version);
        }
    }
}