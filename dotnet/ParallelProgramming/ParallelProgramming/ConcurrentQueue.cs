using System;
using System.Threading;

namespace ParallelProgramming
{    
    
    public class ConcurrentQueue<T>
    {
        class Entry
        {
            internal readonly T Value;
            internal Entry Next;
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

        public ConcurrentQueue()        
        {
            var marker = new Entry(default(T), null, isAlive: false);
            head = marker;
            tail = marker;

        }


        public void Enqueue(T v)
        {
            var chunk = new Entry(v, null);
            while (true)
            {
                var t = tail;
                var nxt = t.Next;
                if (nxt != null)
                {
                    Interlocked.CompareExchange(ref tail, nxt, t);
                    //continue
                }
                else
                {
                    if (null == Interlocked.CompareExchange(ref tail.Next, chunk, null))
                        return;
                }
            }
        }

        
        public bool Dequeue(out T res)
        {
            while (true)
            {
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