using System;
using System.Diagnostics.Contracts;

namespace ParallelProgramming.DataStructures.Immutable
{
    public struct ImmutableStack<T>
    {
        class Entry
        {
            internal readonly T Value;
            internal readonly Entry Next;

            internal Entry(T value, Entry next)
            {
                Value = value;
                Next = next;
            }
        }

        private readonly Entry head;
        private ImmutableStack(Entry head)
        {
            this.head = head;
        }


        #region API

        [Pure]
        public  bool IsEmpty => head == null;
        
        [Pure]
        public ImmutableStack<T> Push(T value)
        {
            return new ImmutableStack<T> (new Entry(value, head));
        }
        
        [Pure]
        public ImmutableStack<T> Pop(out T value)
        {
            if (head == null) throw new InvalidOperationException("head is null");

            value = head.Value;
            return new ImmutableStack<T> (head.Next);
        }
        

        #endregion
        
    }
}