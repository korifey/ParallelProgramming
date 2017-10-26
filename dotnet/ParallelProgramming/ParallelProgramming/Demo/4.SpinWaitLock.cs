using System;
using System.Threading;

namespace ParallelProgramming.Demo
{
    class TasLock
    {
        private int state = 0;

        public CriticalSection UsingLock()
        {
            while (true)
            {
                if (Interlocked.Exchange(ref state, 1) == 0)
                {
                    return new CriticalSection(this);
                }
            }
        }

        internal struct CriticalSection : IDisposable
        {
            private readonly TasLock owner;

            public CriticalSection(TasLock owner)
            {
                this.owner = owner;
            }

            public void Dispose()
            {
                Interlocked.Exchange(ref owner.state, 0);
            }
        }
    }
    
    
    
    
}