using System.Threading;

namespace ParallelCore.Demo
{
    public class Demo1
    {
        public static void Do()
        {
            
            //todo wait for this thread
            var thread = new Thread(() =>
            {
                Thread.Sleep(60_000);
            }) ;
            thread.Start();
        }
    }
}