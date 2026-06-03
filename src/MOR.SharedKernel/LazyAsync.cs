namespace System
{
    // https://codeopinion.com/lazy-async/

    public class LazyAsync<T> : Lazy<Task<T>>
    {
        public LazyAsync(Func<Task<T>> taskFunc)
            : base(() => Task.Factory.StartNew(taskFunc).Unwrap())
        {
        }

        public LazyAsync(Func<Task<T>> taskFunc, System.Threading.LazyThreadSafetyMode mode)
            : base(() => Task.Factory.StartNew(taskFunc).Unwrap(), mode)
        {
        }
    }
}
