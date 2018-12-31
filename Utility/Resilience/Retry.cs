namespace Utility
{
    using System;
    using System.Collections.Generic;
    using System.Threading;

    /// <summary>
    /// Retry routine in case a method is failed. 
    /// This is useful when API connection closed due to unstable environment
    /// 
    /// how to use 
    /// #1
    /// Retry.Do(process.Run, TimeSpan.FromMinutes(5));
    /// #2
    /// Retry.Do(() => MethodToRetry(), retryInterval);
    /// 
    /// </summary>
    public static class Retry
    {
        public static void Do(Action action, TimeSpan retryInterval, int maxRetry = 3, DateTime timeLimit = new DateTime())
        {
            Do<object>(() =>
            {
                action();
                return null;
            }, retryInterval, maxRetry);    
        }

        public static TResult Do<TResult>(Func<TResult> func, TimeSpan retryInterval, int maxRetry = 3, DateTime timeLimit = new DateTime())
        {
            var exceptions = new List<Exception>();
            var defaultTime = new DateTime();

            for (int retry = 0; retry < maxRetry; retry++)
            {
                try
                {
                    // if there is time limit, retry within time limit
                    if (timeLimit == defaultTime || DateTime.Now < timeLimit)
                    {
                        return func();
                    }
                }
                catch (Exception ex)
                {
                    exceptions.Add(ex);
                    Thread.Sleep(retryInterval);
                }
            }

            throw new AggregateException(exceptions);
        }
    }
}
