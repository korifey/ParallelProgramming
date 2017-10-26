using System.Linq;
using System.Runtime.InteropServices;

namespace ParallelProgramming.Util
{
    public class Range
    {
        public readonly int From;
        public readonly int Count;

        public int ToExclusive => From + Count;
        
        public Range(int @from, int count)
        {
            From = @from;
            Count = count;
        }
        
    }
    
    public static class Primes
    {
        public static bool IsPrime(this int thіs)
        {
            for (int i = 2; i * i <= thіs; i++)
            {
                if (thіs % i == 0) return false;
            }

            return true;
        }
        
        
//        static int NumberOfPrimesBefore(int toExclusive)
//        {
//            return NumberOfPrimesBetween(2, toExclusive);
//        }
        
        public static int Between(Range range)
        {
            return Enumerable.Range(range.From, range.Count).Count(IsPrime);
        }
    }
}