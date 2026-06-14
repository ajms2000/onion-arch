namespace System
{
    public static class TaskUtilities
    {
        private static TaskFactory _TFactory = new TaskFactory(
            Threading.CancellationToken.None,
            TaskCreationOptions.None,
            TaskContinuationOptions.None,
            TaskScheduler.Default);


        public static TResult RunSync<TResult>(Func<Task<TResult>> func)
        {
            return _TFactory.StartNew(func).Unwrap().GetAwaiter().GetResult();
        }

        public static void RunSync(Func<Task> func)
        {
            _TFactory.StartNew(func).Unwrap().GetAwaiter().GetResult();
        }

        public static T GetAwaitResult<T>(this Task<T> task)
        {
            var ret = task.ConfigureAwait(false).GetAwaiter().GetResult();
            return ret;
        }

        public static void GetAwaitResult(this Task task)
        {
            task.ConfigureAwait(false).GetAwaiter().GetResult();
        }

        public static T GetAwaitResult<T>(this ValueTask<T> task)
        {
            var ret = task.ConfigureAwait(false).GetAwaiter().GetResult();
            return ret;
        }

        public static void GetAwaitResult(this ValueTask task)
        {
            task.ConfigureAwait(false).GetAwaiter().GetResult();
        }
    }
}
