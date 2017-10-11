namespace ParallelProgramming
{
    public static class Boxed
    {
        public static Boxed<T> Of<T>(T value)
        {
            return new Boxed<T>(value);
        } 
    }
    
    public class Boxed<T>
    {
        public T Value
        {
            get;
        }
        
        public Boxed(T value)
        {
            Value = value;
        }                
    }
}