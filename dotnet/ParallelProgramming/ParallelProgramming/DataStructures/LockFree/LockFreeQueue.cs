using System;
using System.Threading;

namespace ParallelProgramming.DataStructures.LockFree
{    
    
    public class LockFreeQueue<T> : QueueBase<T>
    {
        class Entry
        {
            internal readonly T Value;
            internal Entry Next;
            //logically removed
            internal bool IsAlive => AliveState != 0;
            internal int AliveState;
            
            public Entry(T value, Entry next, bool isAlive = true)
            {
                Value = value;
                Next = next;
                AliveState = isAlive ? 1 : 0;
            }
        }

        private Entry head;
        private Entry tail;

        public LockFreeQueue()        
        {
            var marker = new Entry(default(T), null, isAlive: false);
            head = marker;
            tail = marker;

        }


        //never return false
        public override bool TryEnqueue(T v)
        {
            var chunk = new Entry(v, null);
            while (true)
            {
                var t = tail;
                var nxt = t.Next;
                if (nxt != null)
                {
                    //try to set tail to pred-null element 
                    Interlocked.CompareExchange(ref tail, nxt, t);
                    //if failed, then tail was moved by helper thread
                    //continue
                }
                else
                {                     
                    if (null == Interlocked.CompareExchange(ref tail.Next, chunk, null))
                        return true;
                    //try again, somebody enqueued something
                }
            }
        }

        
        public override bool TryDequeue(out T res)
        {
            while (true)
            {
                //invariant - head is always points to last "logically removed" entry 
                var h = head;
                if (h.IsAlive) throw new InvalidOperationException("head.isAlive");
                
                var nxt = h.Next;
                if (nxt == null)
                {
                    res = default(T);
                    return false;
                }
                
                if (nxt.IsAlive && Interlocked.CompareExchange(ref nxt.AliveState, 0, 1) == 1)
                {
                    res = nxt.Value;
                    return true;

                }
                
                Interlocked.CompareExchange(ref head, nxt, h);
            }
        }
        
    }
}