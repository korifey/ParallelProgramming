using System;
using System.Diagnostics;
using System.Threading;
using ParallelCore.Demo;

namespace ParallelCore
{
    public class Program
    {
        public static void Main(string[] args)
        {
            Console.WriteLine($"ProcessorCount={Environment.ProcessorCount}, " +
                              $"PriorityKind={Process.GetCurrentProcess().BasePriority}, " +
                              $"ThreadPriority={Thread.CurrentThread.Priority}");

//            Demo1.Do();
            Demo3StaContext.Do();
        }
    }
}