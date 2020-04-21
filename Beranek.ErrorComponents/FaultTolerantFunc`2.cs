namespace Beranek.ErrorComponents
{
    using System;
    public class FaultTolerantFunc<T, TResult>
    {
        private const int DefaultDelay = 1000;
        protected Func<T, TResult> MyFunc { get; set; }
        protected int MaxTries { get; set; } = 10;
        protected IRetryStrategy Strategy { get; set; }

        public FaultTolerantFunc()
        { }

        public FaultTolerantFunc(Func<T, TResult> fun)
        {
            MyFunc = fun;
            Strategy = new LinearRetryStrategy(DefaultDelay);
        }

        public FaultTolerantFunc(Func<T, TResult> fun, int maxTries)
        {
            MyFunc = fun;
            MaxTries = maxTries;
            Strategy = new LinearRetryStrategy(DefaultDelay);
        }

        public FaultTolerantFunc(Func<T, TResult> fun, int maxTries, IRetryStrategy strategy)
        {
            MyFunc = fun;
            MaxTries = maxTries;
            Strategy = strategy;
        }

        public TResult Invoke(T arg)
        {
            bool tryAgain = true;
            TResult result = default(TResult);

            using (var retryAction = new DelayedFunc<T, TResult>(MyFunc))
            {
                while (tryAgain)
                {
                    Tries++;
                    retryAction.SetParameter(arg);
                    var awaiter = retryAction.RunWithDelayAsync(Strategy.GetNextDelay())
                        .ConfigureAwait(true)
                        .GetAwaiter();

                    try
                    {
                        result = awaiter.GetResult();
                    }
                    catch (Exception e)
                    {
                        Exception = e;
                    }

                    tryAgain = Tries < MaxTries;
                }

                retryAction.Cancel();
            }

            return result;
        }

        public Exception Exception { get; private set; }

        public int Tries { get; private set; }
    }
}

