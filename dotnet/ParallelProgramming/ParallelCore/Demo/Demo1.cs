using System.Threading;

namespace ParallelCore.Demo
{
    public class Demo1
    {
        public static void Do()
        {
            
            var thread = new Thread(() =>
            {
                Thread.Sleep(60_000);
            }) ;
            thread.Start();
        }
    }
}