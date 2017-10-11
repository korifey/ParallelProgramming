using System;

namespace ParallelProgramming
{
    public static class ThreadName
    {
        [ThreadStatic] 
        public static string Value;
    }
}